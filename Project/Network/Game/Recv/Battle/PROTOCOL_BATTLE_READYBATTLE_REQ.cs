using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_READYBATTLE_REQ : GamePacketReader
    {
        private int espectadorType;
        public override void ReadImplement()
        {
            espectadorType = ReadInt(); //0 - NORMAL || 1 - OBSERVER (GM)
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                DateTime now = DateTime.Now;
                if (espectadorType < 0 || espectadorType > 1 || room == null || room.GetLeader() == null || !room.GetChannel(out Channel channel) || !room.GetSlot(player.slotId, out Slot slot) || (now - player.lastReadyBattle).TotalSeconds < 0.7)
                {
                    return;
                }
                if (slot.equipment == null)
                {
                    client.SendCompletePacket(PackageDataManager.BATTLE_READY_ERROR_0x800010AB_PAK);
                    return;
                }
                bool isBotMode = room.IsBotMode();
                if (!isBotMode)
                {
                    if (CheckTournamentRules(player, room.roomName.ToUpper()))
                    {
                        return;
                    }
                    if (player.access == AccessLevelEnum.TransmissionChampionships && espectadorType == 0)
                    {
                        client.SendPacket(new LOBBY_CHATTING_PAK("Server", player.GetSessionId(), 0, true, "Está conta é destinada apenas para transmissão de campeonatos, ative o modo observador para iniciar!"));
                        return;
                    }
                }
                slot.specGM = espectadorType == 1 && player.IsGM();
                int TotalEnemys = 0;
                int redPlayers = 0;
                int bluePlayers = 0;
                GetReadyPlayers(room, ref redPlayers, ref bluePlayers, ref TotalEnemys);
                if (room.leaderSlot == player.slotId)
                {
                    if (room.state != RoomStateEnum.Ready && room.state != RoomStateEnum.CountDown)
                    {
                        return;
                    }
                    if (room.stage4vs4 == 1)
                    {
                        Check4vs4(room, true, ref redPlayers, ref bluePlayers, ref TotalEnemys);
                    }
                    if (ClanMatchCheck(room, channel.type, TotalEnemys))
                    {
                        return;
                    }
                    if (TotalEnemys == 0 && (isBotMode || room.mode == RoomTypeEnum.Tutorial)) //MODO SPECIAL OU TUTORIAL
                    {
                        room.ChangeSlotState(slot, SlotStateEnum.READY, false);
                        room.StartBattle(false);
                        room.UpdateSlotsInfo();
                    }
                    else if (!isBotMode && TotalEnemys > 0)
                    {
                        room.ChangeSlotState(slot, SlotStateEnum.READY, false);
                        if (room.balancing != BalancingTeamEnum.DISABLED)
                        {
                            TryBalanceTeams(room);
                        }
                        if (room.ThisModeHaveCountDown())
                        {
                            if (room.state == RoomStateEnum.Ready)
                            {
                                room.state = RoomStateEnum.CountDown;
                                room.UpdateRoomInfo();
                                room.StartCountDown();
                            }
                            else if (room.state == RoomStateEnum.CountDown)
                            {
                                room.ChangeSlotState(room.leaderSlot, SlotStateEnum.NORMAL, false);
                                room.StopCountDown(CountDownEnum.StopByHost);
                            }
                        }
                        else
                        {
                            room.StartBattle(false);
                        }
                        room.UpdateSlotsInfo();
                    }
                    else if (TotalEnemys == 0)
                    {
                        client.SendCompletePacket(PackageDataManager.BATTLE_READY_ERROR_0x80001009_PAK);
                    }
                }
                else if (room.slots[room.leaderSlot].state >= SlotStateEnum.LOAD && room.IsPreparing()) //SE O DONO DA SALA JA INICIOU A PARTIDA
                {
                    if (slot.state == SlotStateEnum.NORMAL)
                    {
                        if (room.stage4vs4 == 1 && Check4vs4(room, false, ref redPlayers, ref bluePlayers, ref TotalEnemys))
                        {
                            client.SendCompletePacket(PackageDataManager.BATTLE_4VS4_ERROR_PAK);
                            return;
                        }
                        if (!isBotMode && room.balancing != BalancingTeamEnum.DISABLED)
                        {
                            room.TryBalancePlayer(player, true, ref slot);
                        }
                        room.ChangeSlotState(slot, SlotStateEnum.LOAD, true);
                        slot.SetMissionsClone(player.missions);
                        client.SendPacket(new BATTLE_READYBATTLE_PAK(room));
                        client.SendPacket(new BATTLE_READY_ERROR_PAK((uint)slot.state));
                        using (BATTLE_READYBATTLE2_PAK packet = new BATTLE_READYBATTLE2_PAK(slot, player.titles))
                        {
                            room.SendPacketToPlayers(packet, SlotStateEnum.READY, 1, slot.Id);
                        }
                    }
                }
                else if (slot.state == SlotStateEnum.NORMAL)
                {
                    room.ChangeSlotState(slot, SlotStateEnum.READY, true);
                }
                else if (slot.state == SlotStateEnum.READY)
                {
                    room.ChangeSlotState(slot, SlotStateEnum.NORMAL, false);
                    if (room.state == RoomStateEnum.CountDown && room.GetPlayingPlayers(room.leaderSlot % 2 == 0 ? 1 : 0, SlotStateEnum.READY, 0) == 0)
                    {
                        room.ChangeSlotState(room.leaderSlot, SlotStateEnum.NORMAL, false);
                        room.StopCountDown(CountDownEnum.StopByPlayer);
                    }
                    room.UpdateSlotsInfo();
                }
                player.lastReadyBattle = now;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }

        private void GetReadyPlayers(Room room, ref int redPlayers, ref int bluePlayers, ref int TotalEnemys)
        {
            lock (room.slots)
            {
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = room.slots[i];
                    if (slot.state == SlotStateEnum.READY)
                    {
                        if (slot.teamId == 0)
                        {
                            redPlayers++;
                        }
                        else
                        {
                            bluePlayers++;
                        }
                    }
                }
                if (room.leaderSlot % 2 == 0)
                {
                    TotalEnemys = bluePlayers;
                }
                else
                {
                    TotalEnemys = redPlayers;
                }
            }
        }

        private bool ClanMatchCheck(Room room, int type, int TotalEnemys)
        {
            if (type != 4)
            {
                return false;
            }
            if (!room.Have2ClansToClanMatch())
            {
                client.SendCompletePacket(PackageDataManager.BATTLE_READY_ERROR_0x80001071_PAK); //STBL_IDX_EP_ROOM_NO_START_FOR_NO_CLAN_TEAM
                return true;
            }
            if (TotalEnemys > 0 && !room.HavePlayersToClanMatch())
            {
                client.SendCompletePacket(PackageDataManager.BATTLE_READY_ERROR_0x80001072_PAK); //STBL_IDX_EP_ROOM_NO_START_FOR_TEAM_NOTFULL
                return true;
            }
            return false;
        }

        private void TryBalanceTeams(Room room) //Balanceia os players com READY entre os times. (QTY/RANK)
        {
            lock (room.slots)
            {
                if (room.balancing == BalancingTeamEnum.QTY)
                {
                    List<SlotChange> changeList = new List<SlotChange>();
                    foreach (int slotId in room.RED_TEAM)
                    {
                        Slot CurrentSlot = room.GetSlot(slotId);
                        if (CurrentSlot.playerId > 0 && CurrentSlot.state == SlotStateEnum.READY)
                        {
                            if (room.GetPlayerBySlot(CurrentSlot, out Account player))
                            {
                                foreach (int newSlotId in room.RED_TEAM)
                                {
                                    if (newSlotId < slotId)
                                    {
                                        Slot newSlot = room.slots[newSlotId];
                                        if (newSlot.state == SlotStateEnum.EMPTY && newSlot.playerId == 0)
                                        {
                                            newSlot.state = SlotStateEnum.READY;
                                            newSlot.playerId = player.playerId;
                                            newSlot.equipment = player.equipments;
                                            if (player.slotId == room.leaderSlot)
                                            {
                                                room.leaderSlot = newSlotId;
                                            }
                                            player.slotId = newSlotId;

                                            CurrentSlot.ResetSlot();
                                            CurrentSlot.state = SlotStateEnum.EMPTY;
                                            CurrentSlot.playerId = 0;
                                            CurrentSlot.equipment = null;

                                            changeList.Add(new SlotChange(CurrentSlot, newSlot));
                                            Logger.Red($" TEAM RED BALANCE [!] Nick: {player.nickname} OldSlot: {CurrentSlot.Id} NewSlot: {newSlot.Id}");
                                            break;
                                        }
                                        //else if (newSlot.state > SlotStateEnum.CLOSE && newSlot.state < SlotStateEnum.READY)
                                        //{
                                        //    if (room.GetPlayerBySlot(CurrentSlot, out Account playerSlot))
                                        //    {
                                        //        CurrentSlot.ResetSlot();
                                        //        CurrentSlot.state = newSlot.state;
                                        //        CurrentSlot.playerId = playerSlot.playerId;
                                        //        CurrentSlot.equipment = player.equipments;
                                        //        playerSlot.slotId = CurrentSlot.Id;

                                        //        newSlot.ResetSlot();
                                        //        newSlot.state = SlotStateEnum.READY;
                                        //        newSlot.playerId = player.playerId;
                                        //        newSlot.equipment = player.equipments;
                                        //        if (player.slotId == room.leaderSlot)
                                        //        {
                                        //            room.leaderSlot = newSlotId;
                                        //        }
                                        //        player.slotId = newSlotId;
                                        //        changeList.Add(new SlotChange(CurrentSlot, newSlot));
                                        //    }
                                        //}
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    foreach (int slotId in room.BLUE_TEAM)
                    {
                        Slot CurrentSlot = room.GetSlot(slotId);
                        if (CurrentSlot.playerId > 0 && CurrentSlot.state == SlotStateEnum.READY)
                        {
                            if (room.GetPlayerBySlot(CurrentSlot, out Account player))
                            {
                                foreach (int newSlotId in room.BLUE_TEAM)
                                {
                                    if (newSlotId < slotId)
                                    {
                                        Slot newSlot = room.slots[newSlotId];
                                        if (newSlot.state == SlotStateEnum.EMPTY && newSlot.playerId == 0)
                                        {
                                            newSlot.state = SlotStateEnum.READY;
                                            newSlot.playerId = player.playerId;
                                            newSlot.equipment = player.equipments;
                                            if (player.slotId == room.leaderSlot)
                                            {
                                                room.leaderSlot = newSlotId;
                                            }
                                            player.slotId = newSlotId;

                                            CurrentSlot.ResetSlot();
                                            CurrentSlot.state = SlotStateEnum.EMPTY;
                                            CurrentSlot.playerId = 0;
                                            CurrentSlot.equipment = null;
                                            changeList.Add(new SlotChange(CurrentSlot, newSlot));
                                            Logger.Blue($" TEAM BLUE BALANCE [!] Nick: {player.nickname} OldSlot: {CurrentSlot.Id} NewSlot: {newSlot.Id}");
                                            break;
                                        }
                                        //else if (newSlot.state > SlotStateEnum.CLOSE && newSlot.state < SlotStateEnum.READY)
                                        //{
                                        //    if (room.GetPlayerBySlot(CurrentSlot, out Account playerSlot))
                                        //    {
                                        //        CurrentSlot.ResetSlot();
                                        //        CurrentSlot.state = newSlot.state;
                                        //        CurrentSlot.playerId = playerSlot.playerId;
                                        //        CurrentSlot.equipment = player.equipments;
                                        //        playerSlot.slotId = CurrentSlot.Id;

                                        //        newSlot.ResetSlot();
                                        //        newSlot.state = SlotStateEnum.READY;
                                        //        newSlot.playerId = player.playerId;
                                        //        newSlot.equipment = player.equipments;
                                        //        if (player.slotId == room.leaderSlot)
                                        //        {
                                        //            room.leaderSlot = newSlotId;
                                        //        }
                                        //        player.slotId = newSlotId;
                                        //        changeList.Add(new SlotChange(CurrentSlot, newSlot));
                                        //    }
                                        //}
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (changeList.Count > 0)
                    {
                        using (PROTOCOL_ROOM_CHANGE_SLOTS_ACK packet = new PROTOCOL_ROOM_CHANGE_SLOTS_ACK(changeList, room.leaderSlot, 1))
                        {
                            room.SendPacketToPlayers(packet);
                        }
                        room.UpdateSlotsInfo();
                    }
                }
            }
            for (int i = 0; i < 16; i++)
            {
                Slot slot = room.GetSlot(i);
                if (i != room.leaderSlot && slot.playerId > 0 && slot.state == SlotStateEnum.READY && room.GetPlayerBySlot(slot, out Account player))
                {
                    room.TryBalancePlayer(player, false, ref slot);
                }
            }
        }

        private bool Check4vs4(Room room, bool isLeader, ref int redPlayers, ref int bluePlayers, ref int TotalEnemys)
        {
            if (!isLeader)
            {
                if ((redPlayers + bluePlayers) >= 8) //readPlayers >= 4 || bluePlayers >= 4 se esse maneira não funcionar
                {
                    return true;
                }
                return false;
            }
            int readyPlayers = redPlayers + bluePlayers + 1;
            if (readyPlayers > 8)
            {
                int diference = readyPlayers - 8;
                if (diference > 0)
                {
                    for (int i = 15; i >= 0; i--)
                    {
                        if (i != room.leaderSlot)
                        {
                            Slot slot = room.slots[i];
                            if (slot != null && slot.state == SlotStateEnum.READY)
                            {
                                Account account = room.GetPlayerBySlot(slot);
                                if (account != null)
                                {
                                    room.ChangeSlotState(i, SlotStateEnum.NORMAL, false);
                                    account.SendCompletePacket(PackageDataManager.BATTLE_4VS4_ERROR_PAK);
                                    if (i % 2 == 0)
                                    {
                                        --redPlayers;
                                    }
                                    else
                                    {
                                        --bluePlayers;
                                    }
                                    if ((--diference) == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    room.UpdateSlotsInfo();
                    if (room.leaderSlot % 2 == 0)
                    {
                        TotalEnemys = bluePlayers;
                    }
                    else
                    {
                        TotalEnemys = redPlayers;
                    }
                    return true;
                }
            }
            return false;
        }

        private bool CheckTournamentRules(Account player, string roomName)
        {
            if (TournamentRulesManager.CheckRoomRule(roomName))
            {
                PlayerEquipedItems equip = player.equipments;
                List<string> blocks = new List<string>();
                if (TournamentRulesManager.IsBlocked(roomName, equip.primary))
                {
                    blocks.Add("Primária");
                }
                if (TournamentRulesManager.IsBlocked(roomName, equip.secondary))
                {
                    blocks.Add("Secundária");
                }
                if (TournamentRulesManager.IsBlocked(roomName, equip.melee))
                {
                    blocks.Add("Arma branca");
                }
                if (TournamentRulesManager.IsBlocked(roomName, equip.grenade))
                {
                    blocks.Add("Granada");
                }
                if (TournamentRulesManager.IsBlocked(roomName, equip.special))
                {
                    blocks.Add("Especial");
                }
                if (TournamentRulesManager.IsBlocked(roomName, equip.red))
                {
                    blocks.Add("Pers. Vermelho");
                }
                if (TournamentRulesManager.IsBlocked(roomName, equip.blue))
                {
                    blocks.Add("Pers. Azul");
                }
                int ItemClassType = Utilities.GetItemIdClass(equip.helmet);
                if (TournamentRulesManager.IsBlocked(roomName, equip.helmet) || ItemClassType == 110400 || ItemClassType == 110500)
                {
                    blocks.Add("Capacete");
                }
                if (TournamentRulesManager.IsBlocked(roomName, equip.beret))
                {
                    blocks.Add("Boina");
                }
                if (blocks.Count > 0)
                {
                    string message = string.Format("Não é possível jogar devido a regra @CAMP.\nItens não aceitos:  {0}", string.Join(", ", blocks.ToArray()));
                    player.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(message));
                    return true;
                }
            }
            return false;
        }
    }
}