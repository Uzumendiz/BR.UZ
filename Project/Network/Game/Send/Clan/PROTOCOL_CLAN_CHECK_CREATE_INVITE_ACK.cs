namespace PointBlank.Game
{
    public class CLAN_CHECK_CREATE_INVITE_PAK : GamePacketWriter
    {
        private uint _erro;
        public CLAN_CHECK_CREATE_INVITE_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1315);
            WriteD(_erro);
        }
    }
}