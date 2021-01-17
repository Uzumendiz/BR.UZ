using System;

namespace PointBlank.Game
{
    public class SHOP_LIST_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2822);
            WriteD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
        }
    }
}