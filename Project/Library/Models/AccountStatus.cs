using System;

namespace PointBlank
{
    public class AccountStatus
    {
        public long playerId;
        public byte channelId;
        public byte roomId;
        public byte clanFId;
        public byte serverId;
        public byte[] buffer = new byte[4];
        public void ResetData(long player_id)
        {
            if (player_id == 0)
            {
                return;
            }
            int channel = channelId, room = roomId, clan = clanFId, server = serverId;
            SetData(4294967295, player_id);
            if (channel != channelId || room != roomId || clan != clanFId || server != serverId)
            {
                Utilities.ExecuteQuery($"UPDATE accounts SET status='{(long)4294967295}' WHERE id='{player_id}'");
            }
        }
        public void SetData(uint uintData, long pId)
        {
            SetData(BitConverter.GetBytes(uintData), pId);
        }
        public void SetData(byte[] buffer, long playerId)
        {
            this.playerId = playerId;
            this.buffer = buffer;
            channelId = buffer[0];
            roomId = buffer[1];
            serverId = buffer[2];
            clanFId = buffer[3];
        }
        public void UpdateChannel(byte channelId)
        {
            this.channelId = channelId;
            buffer[0] = channelId;
            UpdateDB();
        }
        public void UpdateRoom(byte roomId)
        {
            this.roomId = roomId;
            buffer[1] = roomId;
            UpdateDB();
        }
        public void UpdateServer(byte serverId)
        {
            this.serverId = serverId;
            buffer[2] = serverId;
            UpdateDB();
        }
        public void UpdateClanMatch(byte clanFId)
        {
            this.clanFId = clanFId;
            buffer[3] = clanFId;
            UpdateDB();
        }
        private void UpdateDB()
        {
            uint value = BitConverter.ToUInt32(buffer, 0);
            Utilities.ExecuteQuery($"UPDATE accounts SET status='{(long)value}' WHERE id='{playerId}'");
        }
    }
}