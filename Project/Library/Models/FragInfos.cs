using System.Collections.Generic;

namespace PointBlank
{
    public class FragInfos
    {
        public byte killerIdx, killsCount, flag;
        public CharaKillTypeEnum killingType;
        public int weaponId;
        public ushort Score;
        public float x, y, z;
        public List<Frag> frags = new List<Frag>();
        public KillingMessageEnum GetAllKillFlags()
        {
            KillingMessageEnum km = 0;
            for (int i = 0; i < frags.Count; i++)
            {
                Frag frag = frags[i];
                if (!km.HasFlag(frag.killFlag))
                {
                    km |= frag.killFlag;
                }
            }
            return km;
        }
    }
}