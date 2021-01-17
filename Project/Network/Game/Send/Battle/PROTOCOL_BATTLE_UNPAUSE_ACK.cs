namespace PointBlank.Game
{
    public class BATTLE_UNPAUSE_ACK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3424);
            WriteD(0);
        }
    }
}