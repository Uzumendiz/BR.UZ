namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_COPYRIGTH_ACK : GamePacketWriter
    {
        public override void Write()
        {
            string T1 = "BR.UZ Server";
            string T2 = "Server Protection";
            string T3 = "Asymmetric Encryption RSA";
            WriteH(7771);
            WriteD(20190505);
            WriteQ(348768394698436);
            WriteS(T1, T1.Length + 1);
            WriteS(T2, T2.Length + 1);
            WriteS(T3, T3.Length + 1);
        }
    }
}