using System.Collections.Generic;

namespace PointBlank.Game
{
    public class BOX_MESSAGE_DELETE_PAK : GamePacketWriter
    {
        private List<object> objects;
        private uint error;
        public BOX_MESSAGE_DELETE_PAK(List<object> objects, uint error)
        {
            this.objects = objects;
            this.error = error;
        }

        public override void Write()
        {
            WriteH(425);
            WriteD(error);
            WriteC((byte)objects.Count);
            foreach (int obj in objects)
            {
                WriteD(obj);
            }
        }
    }
}