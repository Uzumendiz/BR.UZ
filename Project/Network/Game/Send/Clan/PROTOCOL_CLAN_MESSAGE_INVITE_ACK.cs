namespace PointBlank.Game
{
    public class CLAN_MESSAGE_INVITE_PAK : GamePacketWriter
    {
        private uint _erro;
        public CLAN_MESSAGE_INVITE_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1393);
            WriteD(_erro);
        }
    }
}