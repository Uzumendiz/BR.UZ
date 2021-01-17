using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_TIMERSYNC_REQ : GamePacketReader
    {
        private float HackValue;
        private uint TimeRemaining;
        private byte Ping;
        private int HackType;
        private int Latency;
        private int Round;
        public override void ReadImplement()
        {
            TimeRemaining = ReadUint();
            HackValue = ReadFloat(); //(Jogador Normal: 1)
            Round = ReadByte(); //Round da Partida (Não o round atual do jogador)
            Ping = ReadByte();
            HackType = ReadByte(); //(Jogador Normal: 0)
            Latency = ReadShort();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room == null)
                {
                    return;
                }
                bool isBotMode = room.IsBotMode();
                Slot slot = room.GetSlot(player.slotId);
                if (slot == null || slot.state != SlotStateEnum.BATTLE)
                {
                    return;
                }
                if (HackType != 0)
                {
                    Logger.Analyze($" [BATTLE_TIMERSYNC_REQ] [ANALYZE] HackValue: {HackValue} HackType(int): {HackType} HackType(Enum): {(HackTypeEnum)HackType} Nickname: {player.nickname} Login: {player.login} Ip: {client.GetIPAddress()} PlayerId: {player.playerId}");
                    player.Close(0, true);
                    return;
                }
                int BattleTimeLeft = room.GetInBattleTimeLeft();
                if ((BattleTimeLeft - TimeRemaining) > 2 && TimeRemaining < 0x80000000)
                {
                    Logger.Analyze($" [TIMERSYNC] Jogador desconectado por uso de programas ilegais. BattleTimeLeft/TimeRemaining: {BattleTimeLeft}/{TimeRemaining} Nickname: {player.nickname} PlayerId: {player.playerId}");
                    player.Close(0, true);
                    return;
                }
                if (!isBotMode)
                {
                    slot.ping = Ping;
                    slot.latency = Latency;
                    if (slot.latency >= Settings.MaxBattleLatency)
                    {
                        slot.failLatencyTimes++;
                    }
                    else
                    {
                        slot.failLatencyTimes = 0;
                    }
                    if (slot.failLatencyTimes >= Settings.MaxRepeatLatency)
                    {
                        Logger.Warning($" [ROOM] Player: {player.nickname}/{player.playerId} kicked due to high latency. ({slot.latency}/{Settings.MaxBattleLatency}ms)");
                        client.Close(500);
                        return;
                    }
                    else
                    {
                        DateTime now = DateTime.Now;
                        if ((now - room.lastPingSync).TotalSeconds >= Settings.PingUpdateTimeSeconds)
                        {
                            byte[] pings = new byte[16];
                            for (int i = 0; i < 16; i++)
                            {
                                pings[i] = room.slots[i].ping;
                            }
                            using (BATTLE_SENDPING_PAK packet = new BATTLE_SENDPING_PAK(pings))
                            {
                                room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                            }
                            Logger.InfoPing($" [TIMERSYNC] Nickname: {player.nickname} TimeRemaining: {TimeRemaining} BattleTimeLeft: {BattleTimeLeft} Ping ({Ping} bar) Latency ({Latency} ms) HackType: {HackType} Hack: {HackValue}");
                            room.lastPingSync = now;
                        }
                    }
                }
                room.timeRoom = TimeRemaining;
                slot.lastTimeRoom = TimeRemaining;
                if (room.timeRoom > 0x80000000 && BattleTimeLeft <= 1 && !room.swapRound && CompareRounds(room, Round) && room.state == RoomStateEnum.Battle)
                {
                    EndRound(room, isBotMode);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }

        private bool CompareRounds(Room room, int externValue)
        {
            if (room.mode == RoomTypeEnum.Dino || room.mode == RoomTypeEnum.CrossCounter)
            {
                return room.rounds == externValue;
            }
            else
            {
                return room.rounds == externValue + 1;
            }
        }

        private void EndRound(Room room, bool isBotMode)
        {
            try
            {
                room.swapRound = true;
                if (room.mode == RoomTypeEnum.Dino || room.mode == RoomTypeEnum.CrossCounter)
                {
                    if (room.rounds == 1)
                    {
                        room.rounds = 2;
                        for (int i = 0; i < 16; i++)
                        {
                            Slot slot = room.slots[i];
                            if (slot.state == SlotStateEnum.BATTLE)
                            {
                                slot.killsOnLife = 0;
                                slot.lastKillState = 0;
                                slot.repeatLastState = false;
                            }
                        }
                        List<int> dinos = room.GetDinossaurs(true, -2);
                        using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(room, 2, RoundEndTypeEnum.TimeOut))
                        using (BATTLE_ROUND_RESTART_PAK packet2 = new BATTLE_ROUND_RESTART_PAK(room, dinos, isBotMode))
                        {
                            room.SendPacketToPlayers(packet, packet2, SlotStateEnum.BATTLE, 0);
                        }
                        room.round.StartJob(5250, (callbackState) =>
                        {
                            if (room.state == RoomStateEnum.Battle)
                            {
                                room.BattleStart = DateTime.Now.AddSeconds(5250); 
                                using (PROTOCOL_BATTLE_TIMERSYNC_ACK packet = new PROTOCOL_BATTLE_TIMERSYNC_ACK(room))
                                {
                                    room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                                }
                            }
                            room.swapRound = false;
                            lock (callbackState)
                            {
                                room.round.Timer = null;
                            }
                        });
                    }
                    else if (room.rounds == 2)
                    {
                        room.EndBattle(isBotMode, room.GetWinnerTeam());
                    }
                }
                else if (room.ThisModeHaveRounds())
                {
                    int winner = 1;
                    if (room.mode != RoomTypeEnum.Sabotage)
                    {
                        room.blueRounds++;
                    }
                    else
                    {
                        if (room.Bar1 > room.Bar2)
                        {
                            room.redRounds++;
                            winner = 0;
                        }
                        else if (room.Bar1 < room.Bar2)
                        {
                            room.blueRounds++;
                        }
                        else
                        {
                            winner = 2;
                        }
                    }
                    room.BattleEndRound(winner, RoundEndTypeEnum.TimeOut);
                }
                else
                {
                    List<Account> players = room.GetAllPlayers(SlotStateEnum.READY, 1);
                    if (players.Count > 0)
                    {
                        TeamResultTypeEnum winnerTeam = room.GetWinnerTeam();
                        room.CalculateResult(winnerTeam, isBotMode);
                        using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(room, winnerTeam, RoundEndTypeEnum.TimeOut))
                        {
                            room.GetBattleResult(out ushort missionCompletes, out ushort inBattle, out byte[] resultData);
                            byte[] data = packet.GetCompleteBytes("BATTLE_TIMERSYNC_REQ");
                            for (int i = 0; i < players.Count; i++)
                            {
                                Account player = players[i];
                                if (player != null)
                                {
                                    if (room.slots[player.slotId].state == SlotStateEnum.BATTLE)
                                    {
                                        player.SendCompletePacket(data);
                                    }
                                    player.SendPacket(new PROTOCOL_BATTLE_ENDBATTLE_ACK(player, winnerTeam, inBattle, missionCompletes, isBotMode, resultData));
                                }
                            }
                        }
                    }
                    room.ResetBattleInfo();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}