namespace PointBlank.Game
{
    public class BASE_CHANNEL_ENTER_PAK : GamePacketWriter
    {
        private uint channelIdOrError;
        private string announce;
        public BASE_CHANNEL_ENTER_PAK(int channelIdOrError, string announce)
        {
            this.channelIdOrError = (uint)channelIdOrError;
            this.announce = announce;
        }
        public BASE_CHANNEL_ENTER_PAK(uint channelIdOrError)
        {
            this.channelIdOrError = channelIdOrError;
        }

        public override void Write()
        {
            WriteH(2574);
            WriteD(channelIdOrError);
            if (channelIdOrError >= 0 && channelIdOrError <= 9)
            {
                WriteH((ushort)announce.Length);
                WriteS(announce);
            }
        }
    }
}