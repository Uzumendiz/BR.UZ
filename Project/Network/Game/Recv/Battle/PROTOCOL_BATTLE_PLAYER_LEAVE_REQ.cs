using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_PLAYER_LEAVE_REQ : GamePacketReader
    {
        private bool isFinished;
        private long objId;
        public override void ReadImplement()
        {
            objId = ReadLong(); //O bom perdedor (Unidade) - Se ele Usar o item retorna o OBJ do item.
            //0x18E21C00000000
        }
        public override void RunImplement()
        {
            try
            {
                Logger.DebugPacket(GetType().Name, $"objId: {objId}");
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room == null || room.state < RoomStateEnum.Loading || !room.GetSlot(player.slotId, out Slot slot) || slot.state < SlotStateEnum.LOAD)
                {
                    return;
                }
                bool isBotMode = room.IsBotMode();
                FreepassEffect(player, slot, room, isBotMode);
                if (room.vote.Timer != null && room.votekick != null && room.votekick.victimIdx == slot.Id)
                {
                    room.vote.Timer = null;
                    room.votekick = null;
                    room.SendPacketToPlayers(PackageDataManager.VOTEKICK_CANCEL_VOTE_PAK, SlotStateEnum.BATTLE, 0, slot.Id);
                }
                room.ResetSlotInfo(slot, true);
                int red13 = 0, blue13 = 0, red9 = 0, blue9 = 0;
                for (int i = 0; i < 16; i++)
                {
                    Slot slotR = room.slots[i];
                    if (slotR.state >= SlotStateEnum.LOAD)
                    {
                        if (slotR.teamId == 0)
                        {
                            red9++;
                        }
                        else
                        {
                            blue9++;
                        }
                        if (slotR.state == SlotStateEnum.BATTLE)
                        {
                            if (slotR.teamId == 0)
                            {
                                red13++;
                            }
                            else
                            {
                                blue13++;
                            }
                        }
                    }
                }
                if (slot.Id == room.leaderSlot)
                {
                    if (isBotMode)
                    {
                        if (red13 > 0 || blue13 > 0)
                        {
                            LeaveHostBOT_GiveBattle(room, player);
                        }
                        else
                        {
                            LeaveHostBOT_EndBattle(room, player);
                        }
                    }
                    else if ((room.state == RoomStateEnum.Battle && (red13 == 0 || blue13 == 0)) || (room.state <= RoomStateEnum.PreBattle && (red9 == 0 || blue9 == 0)))
                    {
                        LeaveHostNoBOT_EndBattle(room, player, red13, blue13);
                    }
                    else
                    {
                        LeaveHostNoBOT_GiveBattle(room, player);
                    }
                }
                else if (!isBotMode)
                {
                    if ((room.state == RoomStateEnum.Battle && (red13 == 0 || blue13 == 0)) || (room.state <= RoomStateEnum.PreBattle && (red9 == 0 || blue9 == 0)))
                    {
                        LeavePlayerNoBOT_EndBattle(room, player, red13, blue13);
                    }
                    else
                    {
                        LeavePlayer_QuitBattle(room, player);
                    }
                }
                else
                {
                    LeavePlayer_QuitBattle(room, player);
                }
                client.SendPacket(new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(player, 0));
                FirewallSecurity.RemoveRuleUdp(client.GetIPAddress(), client.SessionPort);
                if (!isFinished && room.state == RoomStateEnum.Battle)
                {
                    room.BattleEndRoundPlayersCount();
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }

        private void FreepassEffect(Account player, Slot slot, Room room, bool isBotMode)
        {
            using (DBQuery query = new DBQuery())
            {
                if (player.bonus.freepass == 0 || player.bonus.freepass == 1 && room.channelType == 4)
                {
                    if (isBotMode || slot.state < SlotStateEnum.BATTLE_READY)
                    {
                        return;
                    }
                    if (player.gold >= 200)
                    {
                        player.gold -= 200;
                        query.AddQuery("gold", player.gold);
                    }
                    query.AddQuery("fights_escapes", ++player.statistics.escapes);
                }
                else// if (ch._type != 4)
                {
                    if (room.state != RoomStateEnum.Battle)
                    {
                        return;
                    }
                    int xp = 0, gold = 0;
                    if (isBotMode)
                    {
                        int level = room.IngameAiLevel * (150 + slot.allDeaths);
                        if (level == 0)
                        {
                            level++;
                        }
                        int reward = slot.score / level;
                        gold += reward;
                        xp += reward;
                    }
                    else
                    {
                        int timePlayed = slot.allKills == 0 && slot.allDeaths == 0 ? 0 : (int)slot.InBattleTime(DateTime.Now);
                        if (room.mode == RoomTypeEnum.Destruction || room.mode == RoomTypeEnum.Suppression)
                        {
                            xp = (int)(slot.score + (timePlayed / 2.5) + (slot.allDeaths * 2.2) + (slot.objetivos * 20));
                            gold = (int)(slot.score + (timePlayed / 3.0) + (slot.allDeaths * 2.2) + (slot.objetivos * 20));
                        }
                        else
                        {
                            xp = (int)(slot.score + (timePlayed / 2.5) + (slot.allDeaths * 1.8) + (slot.objetivos * 20));
                            gold = (int)(slot.score + (timePlayed / 3.0) + (slot.allDeaths * 1.8) + (slot.objetivos * 20));
                        }
                    }
                    xp = xp > Settings.MaxBattleExp ? Settings.MaxBattleExp : xp;
                    gold = gold > Settings.MaxBattleGold ? Settings.MaxBattleGold : gold;
                    if ((player.exp + xp) <= 999999999)
                    {
                        player.exp += xp;
                    }
                    if ((player.gold + gold) <= 999999999)
                    {
                        player.gold += gold;
                    }
                    if (xp > 0)
                    {
                        query.AddQuery("exp", player.exp);
                    }
                    if (gold > 0)
                    {
                        query.AddQuery("gold", player.gold);
                    }
                }
                Utilities.UpdateDB("accounts", "id", player.playerId, query.GetTables(), query.GetValues());
            }
        }

        private void LeaveHostBOT_GiveBattle(Room room, Account p)
        {
            List<Account> players = room.GetAllPlayers(SlotStateEnum.READY, 1);
            if (players.Count == 0)
            {
                return;
            }
            int oldLeader = room.leaderSlot;
            room.SetNewLeader(-1, 12, room.leaderSlot, true);
            using (PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK packet = new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(p, 0))
            using (PROTOCOL_BATTLE_GIVEUPBATTLE_ACK packet2 = new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(room, oldLeader))
            {
                byte[] data = packet.GetCompleteBytes("BATTLE_PLAYER_LEAVE_REQ-1");
                byte[] data2 = packet2.GetCompleteBytes("BATTLE_PLAYER_LEAVE_REQ-2");
                for (int i = 0; i < players.Count; i++)
                {
                    Account pR = players[i];
                    Slot slot = room.GetSlot(pR.slotId);
                    if (slot != null)
                    {
                        if (slot.state >= SlotStateEnum.PRESTART)
                        {
                            pR.SendCompletePacket(data2);
                        }
                        pR.SendCompletePacket(data);
                    }
                }
            }
        }

        private void LeaveHostBOT_EndBattle(Room room, Account p)
        {
            List<Account> players = room.GetAllPlayers(SlotStateEnum.READY, 1);
            if (players.Count > 0)
            {
                using (PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK packet = new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(p, 0))
                {
                    byte[] data = packet.GetCompleteBytes("BATTLE_PLAYER_LEAVE_REQ-3");
                    TeamResultTypeEnum winnerTeam = room.GetWinnerTeam();
                    room.GetBattleResult(out ushort missionCompletes, out ushort inBattle, out byte[] a1);
                    for (int i = 0; i < players.Count; i++)
                    {
                        Account pR = players[i];
                        pR.SendCompletePacket(data);
                        pR.SendPacket(new PROTOCOL_BATTLE_ENDBATTLE_ACK(pR, winnerTeam, inBattle, missionCompletes, true, a1));
                    }
                }
            }
            room.ResetBattleInfo();
        }

        /// <summary>
        /// Falta de usuários para continuar com a partida em andamento.
        /// </summary>
        private void LeaveHostNoBOT_EndBattle(Room room, Account p, int red13, int blue13)
        {
            isFinished = true;
            List<Account> players = room.GetAllPlayers(SlotStateEnum.READY, 1);
            if (players.Count > 0)
            {
                TeamResultTypeEnum winnerTeam = room.GetWinnerTeam(red13, blue13);
                if (room.state == RoomStateEnum.Battle)
                {
                    room.CalculateResult(winnerTeam, false);
                }
                using (PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK packet = new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(p, 0))
                {
                    byte[] data = packet.GetCompleteBytes("BATTLE_PLAYER_LEAVE_REQ-4");
                    room.GetBattleResult(out ushort missionCompletes, out ushort inBattle, out byte[] a1);
                    for (int i = 0; i < players.Count; i++)
                    {
                        Account player = players[i];
                        player.SendCompletePacket(data);
                        player.SendPacket(new PROTOCOL_BATTLE_ENDBATTLE_ACK(player, winnerTeam, inBattle, missionCompletes, false, a1));
                    }
                }
            }
            room.ResetBattleInfo();
        }

        private void LeaveHostNoBOT_GiveBattle(Room room, Account p)
        {
            List<Account> players = room.GetAllPlayers(SlotStateEnum.READY, 1);
            if (players.Count == 0)
            {
                return;
            }
            int oldLeader = room.leaderSlot;
            room.SetNewLeader(-1, room.state == RoomStateEnum.Battle ? 12 : 8, room.leaderSlot, true);
            using (PROTOCOL_BATTLE_GIVEUPBATTLE_ACK packet = new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(room, oldLeader))
            using (PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK packet2 = new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(p, 0))
            {
                byte[] data = packet.GetCompleteBytes("BATTLE_PLAYER_LEAVE_REQ-6");
                byte[] data2 = packet2.GetCompleteBytes("BATTLE_PLAYER_LEAVE_REQ-7");
                for (int i = 0; i < players.Count; i++)
                {
                    Account pR = players[i];
                    if (room.slots[pR.slotId].state >= SlotStateEnum.PRESTART)
                    {
                        pR.SendCompletePacket(data);
                    }
                    pR.SendCompletePacket(data2);
                }
            }
        }

        private void LeavePlayerNoBOT_EndBattle(Room room, Account p, int red13, int blue13)
        {
            isFinished = true;
            TeamResultTypeEnum winnerTeam = room.GetWinnerTeam(red13, blue13);
            List<Account> players = room.GetAllPlayers(SlotStateEnum.READY, 1);
            if (players.Count > 0)
            {
                if (room.state == RoomStateEnum.Battle)
                {
                    room.CalculateResult(winnerTeam, false);
                }
                using (PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK packet = new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(p, 0))
                {
                    byte[] data = packet.GetCompleteBytes("BATTLE_PLAYER_LEAVE_REQ-8");
                    room.GetBattleResult(out ushort missionCompletes, out ushort inBattle, out byte[] a1);
                    for (int i = 0; i < players.Count; i++)
                    {
                        Account pR = players[i];
                        pR.SendCompletePacket(data);
                        pR.SendPacket(new PROTOCOL_BATTLE_ENDBATTLE_ACK(pR, winnerTeam, inBattle, missionCompletes, false, a1));
                    }
                }
            }
            room.ResetBattleInfo();
        }

        private void LeavePlayer_QuitBattle(Room room, Account p)
        {
            using (PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK packet = new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(p, 0))
            {
                room.SendPacketToPlayers(packet, SlotStateEnum.READY, 1);
            }
        }
    }
}