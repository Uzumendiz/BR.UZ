namespace PointBlank.Game
{
    public class BASE_SERVER_CHANGE_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2578);
            WriteD(0); //error < 0 = STBL_IDX_EP_SERVER_FAIL_MOVE
        }
    }
}