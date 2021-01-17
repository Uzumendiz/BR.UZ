namespace PointBlank.Game
{
    public class CLAN_MEMBER_LIST_PAK : GamePacketWriter
    {
        private byte[] array;
        private int erro, page, count;
        public CLAN_MEMBER_LIST_PAK(int page, int count, byte[] array)
        {
            this.page = page;
            this.count = count;
            this.array = array;
        }
        public CLAN_MEMBER_LIST_PAK(int erro)
        {
            this.erro = erro;
        }
        public override void Write()
        {
            WriteH(1309);
            WriteD(erro);
            if (erro < 0)
                return;
            WriteC((byte)page);
            WriteC((byte)count);
            WriteB(array);
        }
    }
}