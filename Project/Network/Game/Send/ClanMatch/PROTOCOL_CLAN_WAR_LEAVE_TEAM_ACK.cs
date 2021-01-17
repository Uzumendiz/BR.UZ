namespace PointBlank.Game
{
    public class CLAN_WAR_LEAVE_TEAM_PAK : GamePacketWriter
    {
        private uint _erro;
        public CLAN_WAR_LEAVE_TEAM_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1551);
            WriteD(_erro);
        }
    }
}