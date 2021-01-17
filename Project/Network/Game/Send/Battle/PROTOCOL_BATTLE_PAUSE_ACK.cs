namespace PointBlank.Game
{
    public class BATTLE_PAUSE_ACK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3422);
            WriteD(0); //Error
            WriteD(1);
        }
    }
}