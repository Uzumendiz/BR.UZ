namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK : GamePacketWriter
    {
        private Account player;
        private int type;
        public PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(Account player, int type)
        {
            this.player = player;
            this.type = type;
        }

        public override void Write()
        {
            if (player == null)
            {
                return;
            }
            WriteH(3385);
            WriteD(player.slotId);
            WriteC((byte)type);
            WriteD(player.exp);
            WriteD(player.rankId);
            WriteD(player.gold);
            WriteD(player.statistics.escapes);
            WriteD(player.statistics.escapes);
        }
    }
}