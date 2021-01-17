namespace PointBlank
{
    public class TitleQ
    {
        public int id;
        public int classId;
        public int medals;
        public int brooch;
        public int blueOrder;
        public int insignia;
        public int rank;
        public byte slot;
        public int req1;
        public int req2;
        public long flag;
        public TitleQ() { }
        public TitleQ(int titleId)
        {
            id = titleId;
            flag = (long)1 << titleId;
        }
    }
}