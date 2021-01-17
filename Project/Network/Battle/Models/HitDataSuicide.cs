namespace PointBlank
{
    public class HitDataSuicide
    {
        public int HitInfo;
        public ushort WeaponInfo;
        public Half3 PlayerPos;
        public byte WeaponSlot;
        public ClassTypeEnum WeaponClass;
        public int WeaponId;
        public int WeaponDamage;
        public CharaDeathEnum DeathType;
        public byte KillerId;
        public int HitPart;
        public int Unk;
        public HitCharaPart2Enum CharaHitPart;
        public void SetData()
        {
            WeaponClass = (ClassTypeEnum)((WeaponInfo >> 32) & 63);
            WeaponId = WeaponInfo >> 6;
            WeaponDamage = HitInfo >> 20;
            DeathType = (CharaDeathEnum)(HitInfo & 15);
            KillerId = (byte)((HitInfo >> 11) & 511);
            HitPart = (HitInfo >> 4) & 63;
            CharaHitPart = (HitCharaPart2Enum)HitPart;
            Unk = (HitInfo >> 10) & 1; //0 = User | 1 = Object
        }
    }
}