namespace PointBlank.Game
{
    public class CLAN_CLOSE_PAK : GamePacketWriter
    {
        private uint _erro;
        public CLAN_CLOSE_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1313);
            WriteD(_erro);
        }
    }
}