using System;

namespace PointBlank.Game
{
    public class CLAN_CLIENT_ENTER_PAK : GamePacketWriter
    {
        private int clanId;
        private ClanAuthorityEnum authority;
        public CLAN_CLIENT_ENTER_PAK(int clanId, ClanAuthorityEnum authority)
        {
            this.clanId = clanId;
            this.authority = authority;
        }

        public override void Write()
        {
            WriteH(1442);
            WriteD(clanId);
            WriteD((int)authority);
            if (clanId == 0 || authority == ClanAuthorityEnum.None)
            {
                WriteD(ClanManager.clans.Count);
                WriteC(170);
                WriteH((ushort)Math.Ceiling(ClanManager.clans.Count / 170d));
                WriteD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
            }
        }
    }
}