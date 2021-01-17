namespace PointBlank.Game
{
    public class AUTH_FIND_USER_PAK : GamePacketWriter
    {
        private uint error;
        private Account player;
        public AUTH_FIND_USER_PAK(Account player, uint error)
        {
            this.player = player;
            this.error = error;
        }

        public override void Write()
        {
            WriteH(298);
            WriteD(error);
            if (error == 0)
            {
                WriteD(player.rankId);
                WriteD(Utilities.GetPlayerStatus(player.status, player.isOnline));
                WriteS(ClanManager.GetClan(player.clanId).name, 17);
                WriteD(player.statistics.fights);
                WriteD(player.statistics.fightsWin);
                WriteD(player.statistics.fightsLost);
                WriteD(player.statistics.fightsDraw);
                WriteD(player.statistics.kills);
                WriteD(player.statistics.headshots);
                WriteD(player.statistics.deaths);
                WriteD(player.statistics.totalfights);
                WriteD(player.statistics.totalkills);
                WriteD(player.statistics.escapes);
                WriteD(player.statistics.fights);
                WriteD(player.statistics.fightsWin);
                WriteD(player.statistics.fightsLost);
                WriteD(player.statistics.fightsDraw);
                WriteD(player.statistics.kills);
                WriteD(player.statistics.headshots);
                WriteD(player.statistics.deaths);
                WriteD(player.statistics.totalfights);
                WriteD(player.statistics.totalkills);
                WriteD(player.statistics.escapes);
                WriteD(player.equipments.primary);
                WriteD(player.equipments.secondary);
                WriteD(player.equipments.melee);
                WriteD(player.equipments.grenade);
                WriteD(player.equipments.special);
                WriteD(player.equipments.red);
                WriteD(player.equipments.blue);
                WriteD(player.equipments.helmet);
                WriteD(player.equipments.beret);
                WriteD(player.equipments.dino);
                WriteH(0);
                WriteC(0);
            }
        }
    }
}