using System;

namespace PointBlank.Game
{
    public class SERVER_MESSAGE_DISCONNECT_PAK : GamePacketWriter
    {
        private uint error;
        private bool hackUse;
        public SERVER_MESSAGE_DISCONNECT_PAK(uint error, bool hackUse)
        {
            this.error = error;
            this.hackUse = hackUse;
        }

        public override void Write()
        {
            WriteH(2062);
            WriteD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
            WriteD(error);
            WriteD(hackUse); //Se for igual a 1, novo writeD (Da DC no cliente, Programa ilegal)
            if (hackUse)
            {
                WriteD(0);
            }
        }
    }
}