using System;

namespace PointBlank.Auth
{
    public class PROTOCOL_SERVER_MESSAGE_DISCONNECT_ACK : GamePacketWriter
    {
        private uint error;
        private bool type;
        public PROTOCOL_SERVER_MESSAGE_DISCONNECT_ACK(uint error, bool HackUse)
        {
            this.error = error;
            type = HackUse;
        }

        public override void Write()
        {
            WriteH(2062);
            WriteD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
            WriteD(error);
            WriteD(type); //Se for igual a 1, novo writeD (Da DC no cliente, Programa ilegal)
            if (type)
            {
                WriteD(0);
            }
        }
    }
}