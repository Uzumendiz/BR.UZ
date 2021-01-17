using System;

namespace PointBlank.Game
{
    public class SHOP_ENTER_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2820);
            WriteD(int.Parse(DateTime.Now.ToString("yyMMddHHmm")));
        }
    }
}