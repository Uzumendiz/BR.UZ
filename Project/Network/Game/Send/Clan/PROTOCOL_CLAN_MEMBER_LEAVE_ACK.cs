namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_MEMBER_LEAVE_ACK : GamePacketWriter
    {
        private long playerId;
        public PROTOCOL_CLAN_MEMBER_LEAVE_ACK(long playerId)
        {
            this.playerId = playerId;
        }

        public override void Write()
        {
            WriteH(1353);
            WriteQ(playerId);
        }
    }
}