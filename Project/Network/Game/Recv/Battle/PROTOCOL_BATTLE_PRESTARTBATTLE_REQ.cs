using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_PRESTARTBATTLE_REQ : GamePacketReader
    {
        private int mapId;
        private int stage4vs4;
        private RoomTypeEnum roomType;
        public override void ReadImplement()
        {
            mapId = ReadShort();
            stage4vs4 = ReadByte();
            roomType = (RoomTypeEnum)ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room == null)
                {
                    Logger.Warning($" [Game] [{GetType().Name}] Room is null.");
                    client.Close(0, false);
                    return;
                }
                if (room.stage4vs4 != stage4vs4 || room.mode != roomType || room.mapId != mapId || !MapsXML.CheckId(mapId))
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x8000100A_ACK);
                    room.ChangeSlotState(player.slotId, SlotStateEnum.NORMAL, true);
                    room.BattleEndPlayersCount(room.IsBotMode());
                    return;
                }
                Slot slot = room.slots[player.slotId];
                if (!room.IsPreparing() || slot.state < SlotStateEnum.LOAD)
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_BATTLE_STARTBATTLE_ACK);
                    room.ChangeSlotState(slot, SlotStateEnum.NORMAL, true);
                    room.BattleEndPlayersCount(room.IsBotMode());
                    slot.StopTiming();
                    return;
                }
                Account leader = room.GetLeader();
                if (leader == null)
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x8000100B_ACK);
                    client.SendPacket(new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(player, 0));
                    room.ChangeSlotState(slot, SlotStateEnum.NORMAL, true);
                    room.BattleEndPlayersCount(room.IsBotMode());
                    slot.StopTiming();
                    return;
                }
                if (player.localIP == new byte[4] || player.localIP.Length == 0 || player.ipAddress.GetAddressBytes() == new byte[4] || player.ipAddress.GetAddressBytes().Length == 0)
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x80001008_ACK);
                    client.SendPacket(new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(player, 0));
                    room.ChangeSlotState(slot, SlotStateEnum.NORMAL, true);
                    room.BattleEndPlayersCount(room.IsBotMode());
                    slot.StopTiming();
                    return;
                }
                client.SessionPort = room.sessionPort;
                FirewallSecurity.AddRuleUdp(client.GetIPAddress(), client.SessionPort);
                byte UdpType = (byte)Settings.UdpType;
                if (slot.Id == room.leaderSlot)
                {
                    room.state = RoomStateEnum.PreBattle;
                    room.UpdateRoomInfo();
                    room.GenerateRoomSeed();
                    //room.LoadHitParts();
                }
                room.ChangeSlotState(slot, SlotStateEnum.PRESTART, true);
                client.SendPacket(new PROTOCOL_BATTLE_PRESTARTBATTLE_ACK(player, leader, true, UdpType));
                if (slot.Id != room.leaderSlot)
                {
                    leader.SendPacket(new PROTOCOL_BATTLE_PRESTARTBATTLE_ACK(player, leader, false, UdpType));
                }
                room.StartCounter(1, player, slot);
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}