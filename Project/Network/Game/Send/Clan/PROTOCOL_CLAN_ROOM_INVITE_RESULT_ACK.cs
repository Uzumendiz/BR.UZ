namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_ROOM_INVITE_RESULT_ACK : GamePacketWriter
    {
        private long playerId;
        public PROTOCOL_CLAN_ROOM_INVITE_RESULT_ACK(long playerId)
        {
            this.playerId = playerId;
        }

        public override void Write()
        {
            WriteH(1383);
            WriteQ(playerId);
        }
    }
}