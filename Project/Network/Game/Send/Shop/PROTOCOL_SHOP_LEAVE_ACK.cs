namespace PointBlank.Game
{
    public class SHOP_LEAVE_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2818);
        }
    }
}