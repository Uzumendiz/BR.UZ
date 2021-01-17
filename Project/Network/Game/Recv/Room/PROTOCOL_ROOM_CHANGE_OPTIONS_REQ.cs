using System;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHANGE_OPTIONS_REQ : GamePacketReader
    {
        private string leaderName;
        private int killtime;
        private byte limit;
        private byte seeConf;
        private short balancing;
        public override void ReadImplement()
        {
            leaderName = ReadString(33);
            killtime = ReadInt();
            limit = ReadByte();
            seeConf = ReadByte();
            balancing = ReadShort();
        }

        public override void RunImplement()
        {
            try
            {
                Logger.DebugPacket(GetType().Name, $"LeaderName: {leaderName} KillTime: {killtime} Limit: {limit} SeeConf: {seeConf} Balancing: {balancing}");
                //if (leaderName.Length >= Settings.NickMinLength && balancing >= 0 && balancing < 3)
                //{

                //}
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room != null && leaderName == player.nickname && room.leaderSlot == player.slotId && room.state == RoomStateEnum.Ready)
                {
                    room.leaderName = leaderName;
                    room.killtime = killtime;
                    room.limit = limit;
                    room.seeConf = seeConf;
                    room.balancing = (BalancingTeamEnum)balancing;
                    using (PROTOCOL_ROOM_CHANGE_OPTIONS_ACK packet = new PROTOCOL_ROOM_CHANGE_OPTIONS_ACK(room))
                    {
                        room.SendPacketToPlayers(packet);
                    }
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}