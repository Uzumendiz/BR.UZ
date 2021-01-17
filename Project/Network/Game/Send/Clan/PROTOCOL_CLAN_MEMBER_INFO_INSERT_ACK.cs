namespace PointBlank.Game
{
    public class CLAN_MEMBER_INFO_INSERT_PAK : GamePacketWriter
    {
        private Account p;
        private ulong status;
        public CLAN_MEMBER_INFO_INSERT_PAK(Account pl)
        {
            p = pl;
            status = Utilities.GetClanStatus(pl.status, pl.isOnline);
        }

        public override void Write()
        {
            WriteH(1351);
            WriteC((byte)(p.nickname.Length + 1));
            WriteS(p.nickname, p.nickname.Length + 1);
            WriteQ(p.playerId);
            WriteQ(status);
            WriteC(p.rankId);
        }
    }
}