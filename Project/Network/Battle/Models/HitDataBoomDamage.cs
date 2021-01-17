using System.Collections.Generic;

namespace PointBlank
{
    public class HitDataBoomDamage
    {
        public byte WeaponSlot;
        public byte DeathType;
        public ushort BoomInfo;
        public ushort GrenadesCount;
        public ushort WeaponInfo;
        public int HitInfo;
        public int WeaponId;
        public int WeaponDamage;
        public int WeaponObjectId;
        public int HitPart;
        public ObjectTypeEnum ObjectType;
        public List<int> BoomPlayers;
        public Half3 FirePos;
        public Half3 HitPos;
        public HitTypeEnum HitType;
        public ClassTypeEnum WeaponClass;
        public float Range; //Alcance, HitDistance
        public HitCharaPart2Enum CharaHitPart;
        public void SetData()
        {
            HitType = (HitTypeEnum)((WeaponInfo >> 17) & 7);
            ObjectType = (ObjectTypeEnum)(HitInfo & 3);
            WeaponClass = (ClassTypeEnum)(WeaponInfo & 63);
            WeaponId = WeaponInfo >> 6;
            WeaponDamage = HitInfo >> 21;
            WeaponObjectId = (HitInfo >> 2) & 511;
            HitPart = (HitInfo >> 11) & 63;
            CharaHitPart = (HitCharaPart2Enum)HitPart;
            Range = Vector3.DistanceRange(FirePos, HitPos);
        }
    }
}
