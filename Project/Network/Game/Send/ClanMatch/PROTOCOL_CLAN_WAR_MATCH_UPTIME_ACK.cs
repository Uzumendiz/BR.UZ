namespace PointBlank.Game
{
    public class CLAN_WAR_MATCH_UPTIME_PAK : GamePacketWriter
    {
        private int _f;
        private uint _erro;
        public CLAN_WAR_MATCH_UPTIME_PAK(uint erro, int formacao = 0)
        {
            _erro = erro;
            _f = formacao;
        }

        public override void Write()
        {
            WriteH(1572);
            WriteD(_erro);
            if (_erro == 0)
                WriteC((byte)_f);
        }
    }
}