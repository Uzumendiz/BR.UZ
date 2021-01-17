namespace PointBlank.Game
{
    public class CLAN_COMMISSION_MASTER_PAK : GamePacketWriter
    {
        private uint _erro;
        public CLAN_COMMISSION_MASTER_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1338);
            WriteD(_erro);
        }
    }
}