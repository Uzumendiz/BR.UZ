namespace PointBlank.Game
{
    public class CLAN_WAR_MATCH_PROPOSE_PAK : GamePacketWriter
    {
        private uint _erro;
        public CLAN_WAR_MATCH_PROPOSE_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1554);
            WriteD(_erro);
        }
    }
}