using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_CHATTING_REQ : GamePacketReader
    {
        private string text;
        private ChattingTypeEnum type;
        public override void ReadImplement()
        {
            type = (ChattingTypeEnum)ReadShort();
            text = ReadString(ReadShort());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                DateTime now = DateTime.Now;
                if (player == null || type < ChattingTypeEnum.All || type > ChattingTypeEnum.Clan_Member_Page || text.Length > 60 || player.nickname.Length < Settings.NickMinLength || player.nickname.Length  > Settings.NickMaxLength || (now - player.lastChatting).TotalSeconds < 1)
                {
                    return;
                }
                Room room = player.room;
                Slot sender = null;
                if (type == ChattingTypeEnum.Team)
                {
                    if (room == null)
                    {
                        return;
                    }
                    Logger.ChatAll($"[{player.nickname}] [Team] {text}");
                    if (StringFilter.CheckFilterChat(text))
                    {
                        client.SendPacket(new LOBBY_CHATTING_PAK("Server", player.GetSessionId(), 0, true, "Não é possivel digitar palavras inapropriadas."));
                        return;
                    }
                    sender = room.slots[player.slotId];
                    int[] array = room.GetTeamArray(sender.teamId);
                    using (PROTOCOL_ROOM_CHATTING_ACK packet = new PROTOCOL_ROOM_CHATTING_ACK((int)type, sender.Id, player.UseChatGM(), text))
                    {
                        byte[] data = packet.GetCompleteBytes("CHAT_NORMAL_REQ-1");
                        lock (room.slots)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                Slot receiver = room.slots[array[i]];
                                Account playerSlot = room.GetPlayerBySlot(receiver);
                                if (playerSlot != null && SlotValidMessage(sender, receiver))
                                {
                                    playerSlot.SendCompletePacket(data);
                                }
                            }
                        }
                    }
                }
                else if (type == ChattingTypeEnum.All || type == ChattingTypeEnum.Lobby)
                {
                    if (room != null)
                    {
                        Logger.ChatAll($"[{player.nickname}] [Room] (Id: {room.roomId}) {text}");
                        if (ServerCommands(player, room))
                        {
                            client.SendPacket(new PROTOCOL_ROOM_CHATTING_ACK((int)type, player.slotId, true, text));
                            return;
                        }
                        if (StringFilter.CheckFilterChat(text))
                        {
                            client.SendPacket(new LOBBY_CHATTING_PAK("Server", player.GetSessionId(), 0, true, "Não é possivel digitar palavras inapropriadas."));
                            return;
                        }
                        sender = room.slots[player.slotId];
                        using (PROTOCOL_ROOM_CHATTING_ACK packet = new PROTOCOL_ROOM_CHATTING_ACK((int)type, sender.Id, player.UseChatGM(), text))
                        {
                            byte[] data = packet.GetCompleteBytes("CHAT_NORMAL_REQ-2");
                            lock (room.slots)
                            {
                                for (int i = 0; i < 16; i++)
                                {
                                    Slot receiver = room.slots[i];
                                    Account playerSlot = room.GetPlayerBySlot(receiver);
                                    if (playerSlot != null && SlotValidMessage(sender, receiver))
                                    {
                                        playerSlot.SendCompletePacket(data);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Channel channel = player.GetChannel();
                        if (channel == null)
                        {
                            return;
                        }
                        Logger.ChatAll($"[{player.nickname}] [Lobby] {text}");
                        if (ServerCommands(player, room))
                        {
                            client.SendPacket(new LOBBY_CHATTING_PAK(player, text, true));
                            return;
                        }
                        if (StringFilter.CheckFilterChat(text))
                        {
                            client.SendPacket(new LOBBY_CHATTING_PAK("Server", player.GetSessionId(), 0, true, "Não é possivel digitar palavras inapropriadas."));
                            return;
                        }
                        using (LOBBY_CHATTING_PAK packet = new LOBBY_CHATTING_PAK(player, text))
                        {
                            channel.SendPacketToWaitPlayers(packet);
                        }
                    }
                }
                player.lastChatting = now;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }

        private bool ServerCommands(Account player, Room room)
        {
            try
            {
                if (text.StartsWith("(Ghost)"))
                {
                    text.Replace("(Ghost)", "");
                }
                if (text.StartsWith("(Fantasma)"))
                {
                    text.Replace("(Fantasma)", "");
                }
                string Command = text.Substring(1);
                if (!Settings.ChatCommandsActive || !player.HaveGMLevel())
                {
                    return false;
                }
                if (!text.StartsWith(".") && text.Contains("help"))
                {
                    text = "Olha a merda...capoto o corsa. VRUMMMMM PÁH";
                    return true;
                }
                if (!text.StartsWith("."))
                {
                    return false;
                }
                Logger.ChatCommands($" [CHAT] [Commands] PlayerId: {player.playerId} Nick: {player.nickname} Login: {player.login} Ip: {player.ipAddress.ToString()} Text: {text}");

                AccessLevelEnum access = player.access;
                PacketCommand REQ = null;
                if (Command.StartsWith("HELP") && access >= AccessLevelEnum.Moderator)
                {
                    REQ = new CMD_HELP_REQ(1);
                }

                else if (Command.StartsWith("PCCAFEBASIC ") && access >= AccessLevelEnum.Admin)
                {
                    REQ = new CMD_GAMEMASTER_REQ(Command, 6);
                }
                else if (Command.StartsWith("PCCAFEPLUS ") && access >= AccessLevelEnum.Admin)
                {
                    REQ = new CMD_GAMEMASTER_REQ(Command, 7);
                }
                else if (Command.StartsWith("KICK ") && access >= AccessLevelEnum.Moderator)
                {
                    REQ = new CMD_KICK_REQ(Command, 1);
                }
                else if (Command.StartsWith("KICKALL") && access >= AccessLevelEnum.GameMaster)
                {
                    REQ = new CMD_KICK_REQ(Command, 2);
                }
                else if (Command.StartsWith("AFKKICK") && access >= AccessLevelEnum.Moderator)
                {
                    REQ = new CMD_KICK_REQ(Command, 2);
                }
                else if (Command.StartsWith("ONLINE") && access >= AccessLevelEnum.Moderator)
                {
                    REQ = new CMD_SERVERINFO_REQ(1);
                }
                else if (Command.StartsWith("G1 ") && access >= AccessLevelEnum.Moderator) //ALL
                {
                    REQ = new CMD_ANNOUNCE_REQ(Command, 1);
                }
                else if (Command.StartsWith("G2 ") && access >= AccessLevelEnum.Moderator) //ROOM
                {
                    REQ = new CMD_ANNOUNCE_REQ(Command, 2);
                }
                else if (Command.StartsWith("CASH ") && access >= AccessLevelEnum.GameMaster) 
                {
                    REQ = new CMD_PLAYERINFO_REQ(Command, 1);
                }
                else if (Command.StartsWith("GOLD ") && access >= AccessLevelEnum.GameMaster)
                {
                    REQ = new CMD_PLAYERINFO_REQ(Command, 2);
                }
                else if (Command.StartsWith("RANK ") && access >= AccessLevelEnum.GameMaster)
                {
                    REQ = new CMD_GAMEMASTER_REQ(Command, 3);
                }
                else if (Command.StartsWith("NICK ") && access >= AccessLevelEnum.GameMaster)
                {
                    REQ = new CMD_GAMEMASTER_REQ(Command, 4);
                }
                else if (Command.StartsWith("ADDITEM ") && access >= AccessLevelEnum.GameMaster)
                {
                    REQ = new CMD_GAMEMASTER_REQ(Command, 5);
                }
                else if (Command.StartsWith("GMCOLOR") && access >= AccessLevelEnum.Moderator)
                {
                    REQ = new CMD_GAMEMASTER_REQ(Command, 1);
                }
                else if (Command.StartsWith("ANTIKICK") && access >= AccessLevelEnum.Moderator)
                {
                    REQ = new CMD_GAMEMASTER_REQ(Command, 2);
                }
                else if (Command.StartsWith("END") && access == AccessLevelEnum.Developer)
                {
                    REQ = new CMD_DEVELOPER_REQ(Command, 1);
                }
                else if (Command.StartsWith("ROOMTYPE ") && access == AccessLevelEnum.Developer)
                {
                    REQ = new CMD_DEVELOPER_REQ(Command, 2);
                }
                else if (Command.StartsWith("ROOMSPECIAL ") && access == AccessLevelEnum.Developer)
                {
                    REQ = new CMD_DEVELOPER_REQ(Command, 3);
                }
                else if (Command.StartsWith("ROOMWEAPON ") && access == AccessLevelEnum.Developer)
                {
                    REQ = new CMD_DEVELOPER_REQ(Command, 4);
                }
                else
                {
                    text = "Não foi possivel encontrar o comando digitado.";
                }
                if (REQ != null)
                {
                    if (REQ.Set(player, room))
                    {
                        REQ.RunImplement();
                    }
                    text = REQ.GetResponse();
                    REQ = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                text = "Ocorreu um problema ao executar o comando.";
                Logger.Exception(ex);
                return true;
            }
        }

        /// <summary>
        /// Checa se os slots são válidos para trocarem mensagens na sala.
        /// </summary>
        /// <param name="sender">Remetente</param>
        /// <param name="receiver">Destinatário</param>
        /// <returns></returns>
        private bool SlotValidMessage(Slot sender, Slot receiver)
        {
            return ((sender.state == SlotStateEnum.NORMAL || sender.state == SlotStateEnum.READY) &&
                   (receiver.state == SlotStateEnum.NORMAL || receiver.state == SlotStateEnum.READY)) ||
                   (sender.state >= SlotStateEnum.LOAD && receiver.state >= SlotStateEnum.LOAD &&
                   (receiver.specGM || sender.specGM || sender.deathState.HasFlag(DeadEnum.useChat) || sender.deathState.HasFlag(DeadEnum.isDead) && receiver.deathState.HasFlag(DeadEnum.isDead) || sender.espectador && receiver.espectador || sender.deathState.HasFlag(DeadEnum.isAlive) && receiver.deathState.HasFlag(DeadEnum.isAlive) &&
                   (sender.espectador && receiver.espectador || !sender.espectador && !receiver.espectador)));
        }
    }
}