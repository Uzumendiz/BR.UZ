namespace PointBlank
{
    public class Frag
    {
        public byte victimWeaponClass, hitspotInfo, flag;
        public KillingMessageEnum killFlag;
        public float x, y, z;
        public int VictimSlot;
        public Frag() { }
        public Frag(byte hitspotInfo)
        {
            SetHitspotInfo(hitspotInfo);
        }
        public void SetHitspotInfo(byte value)
        {
            hitspotInfo = value;
            VictimSlot = value & 15;
        }
    }
}