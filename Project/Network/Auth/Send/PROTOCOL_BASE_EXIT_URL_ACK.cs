namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_EXIT_URL_ACK : GamePacketWriter
    {
        private int count;
        private string link;
        public PROTOCOL_BASE_EXIT_URL_ACK(string link)
        {
            count = link.Length > 0 ? 1 : 0;
            this.link = link;
        }

        public override void Write()
        {
            WriteH(2694);
            WriteC((byte)count);
            if (count > 0)
            {
                WriteD(1);
                WriteD((int)ClientLocaleEnum.Brazil);
                WriteS(link, 256);
            } //Só considera o último link válido.
        }
    }
}