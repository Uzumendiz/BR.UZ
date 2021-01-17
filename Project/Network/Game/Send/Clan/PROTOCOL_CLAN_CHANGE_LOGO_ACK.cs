namespace PointBlank.Game
{
    public class CLAN_CHANGE_LOGO_PAK : GamePacketWriter
    {
        private uint logo;
        public CLAN_CHANGE_LOGO_PAK(uint logo)
        {
            this.logo = logo;
        }

        public override void Write()
        {
            WriteH(1371);
            WriteD(logo);
        }
    }
}