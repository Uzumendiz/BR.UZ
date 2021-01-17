using System;

namespace PointBlank
{
    public class ObjectInfo
    {
        public int objectId;
        public int life = 100;
        public int destroyState;
        public AnimModel animation;
        public DateTime useDate;
        public ObjModel info;
        public ObjectInfo(int objectId)
        {
            this.objectId = objectId;
        }
    }
}