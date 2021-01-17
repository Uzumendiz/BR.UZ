using System.Collections.Generic;

namespace PointBlank
{
    public class HitDataNormalDamage
    {
        public byte WeaponSlot;
        public ushort BoomInfo;
        public ushort WeaponInfo;
        public int HitInfo;
        public int WeaponId;
        public int WeaponDamage;
        public int WeaponObjectId;
        public int HitPart;
        public ObjectTypeEnum ObjectType;
        public CharaDeathEnum DeathType;
        public Half3 StartBullet;
        public Half3 EndBullet;
        public List<int> BoomPlayers;
        public HitTypeEnum HitType;
        public ClassTypeEnum WeaponClass;
        public HitCharaPart2Enum CharaHitPart;
        public float Range; //Alcance, HitDistance
        public void SetData()
        {
            HitType = (HitTypeEnum)((HitInfo >> 17) & 7);
            WeaponClass = (ClassTypeEnum)(WeaponInfo & 63);
            ObjectType = (ObjectTypeEnum)(HitInfo & 3);
            WeaponId = WeaponInfo >> 6;
            WeaponDamage = HitInfo >> 21;
            WeaponObjectId = (HitInfo >> 2) & 511;
            HitPart = (HitInfo >> 11) & 63;
            CharaHitPart = (HitCharaPart2Enum)HitPart;
            DeathType = HitPart == 29 ? CharaDeathEnum.HEADSHOT : CharaDeathEnum.DEFAULT; //(CharaDeathEnum)(HitInfo & 15)
            Range = Vector3.DistanceRange(StartBullet, EndBullet);
        }
    }
}