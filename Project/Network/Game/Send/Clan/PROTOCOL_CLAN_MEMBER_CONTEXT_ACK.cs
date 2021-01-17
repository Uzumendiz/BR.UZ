using System;

namespace PointBlank.Game
{
    public class CLAN_MEMBER_CONTEXT_PAK : GamePacketWriter
    {
        private int erro, playersCount;
        public CLAN_MEMBER_CONTEXT_PAK(int erro, int playersCount)
        {
            this.erro = erro;
            this.playersCount = playersCount;
        }
        public CLAN_MEMBER_CONTEXT_PAK(int erro)
        {
            this.erro = erro;
        }
        public override void Write()
        {
            WriteH(1307);
            WriteD(erro);
            if (erro == 0)
            {
                WriteC((byte)playersCount);
                WriteC(14);
                WriteC((byte)Math.Ceiling(playersCount / 14d));
                WriteD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
            }
        }
    }
}