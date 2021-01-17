namespace PointBlank.Game
{
    public class CLAN_WAR_ENEMY_INFO_PAK : GamePacketWriter
    {
        public Match mt;
        public CLAN_WAR_ENEMY_INFO_PAK(Match match)
        {
            mt = match;
        }

        public override void Write()
        {
            WriteH(1574);
            WriteH((short)mt.GetServerInfo());
            WriteC((byte)mt.matchId);
            WriteC((byte)mt.friendId);
            WriteC((byte)mt.formação);
            WriteC((byte)mt.GetCountPlayers());
            WriteD(mt.leader);
            WriteC(0);
            WriteD(mt.clan.id);
            WriteC(mt.clan.rank);
            WriteD(mt.clan.logo);
            WriteS(mt.clan.name, 17);
            WriteT(mt.clan.pontos);
            WriteC(mt.clan.nameColor);
        }
    }
}