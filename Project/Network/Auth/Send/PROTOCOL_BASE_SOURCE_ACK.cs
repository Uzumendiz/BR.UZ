using System;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_SOURCE_ACK : GamePacketWriter
    {
        public override void Write()
        {
            string day = DateTime.Now.ToString("ddd MM yyy");
            string hour = DateTime.Now.ToString("HH:mm:ss");

            WriteH(2679);
            WriteD(8);
            WriteC(1);
            WriteC(5);

            WriteH(1); //versão jogo
            WriteH(15); //versão jogo
            WriteH(42); //versão jogo
            WriteH(30); //versão jogo

            WriteH(1012); //versão udp
            WriteH(12); //versão udp

            WriteC(5);
            WriteS(day, day.Length);
            WriteD(0);
            WriteS(hour, hour.Length);
            WriteB(new byte[7]);
            WriteS("BR.UZ", 5);
            WriteH(0);
        }
    }
}