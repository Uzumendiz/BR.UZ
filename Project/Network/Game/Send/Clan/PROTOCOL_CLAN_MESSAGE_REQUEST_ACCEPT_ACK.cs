namespace PointBlank.Game
{
    public class CLAN_MESSAGE_REQUEST_ACCEPT_PAK : GamePacketWriter
    {
        private uint _erro;
        public CLAN_MESSAGE_REQUEST_ACCEPT_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1395);
            WriteD(_erro);
        }
    }
}