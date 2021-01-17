namespace PointBlank
{
    public class ObjectHitInfo
    {
        public int SyncType;
        public int ObjSyncId;
        public int ObjId;
        public int ObjectLife;
        public int WeaponId;
        public int killerId;
        public int AnimationId1;
        public int AnimationId2;
        public int DestroyState;
        public int HitPart;
        public float SpecialUse;
        public Half3 Position;
        public CharaDeathEnum DeathType = CharaDeathEnum.DEFAULT;
        public ObjectHitInfo(int type)
        {
            SyncType = type;
        }
    }
}