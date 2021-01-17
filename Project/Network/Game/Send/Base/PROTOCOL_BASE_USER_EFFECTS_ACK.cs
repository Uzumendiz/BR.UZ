namespace PointBlank.Game
{
    public class PROTOCOL_BASE_USER_EFFECTS_ACK : GamePacketWriter
    {
        private int type;
        private PlayerBonus bonus;
        public PROTOCOL_BASE_USER_EFFECTS_ACK(int type, PlayerBonus bonus)
        {
            this.type = type;
            this.bonus = bonus;
        }

        public override void Write()
        {
            WriteH(2638);
            WriteH((ushort)type);
            WriteD(bonus.fakeRank);
            WriteD(bonus.fakeRank);
            WriteS(bonus.fakeNick, 33);
            WriteH(bonus.sightColor);
        }
    }
}