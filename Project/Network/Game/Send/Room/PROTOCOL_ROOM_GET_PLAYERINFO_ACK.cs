namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_GET_PLAYERINFO_ACK : GamePacketWriter
    {
        private Account player;
        private bool IsAuthority;
        public PROTOCOL_ROOM_GET_PLAYERINFO_ACK(Account player, bool IsAuthority)
        {
            this.player = player;
            this.IsAuthority = IsAuthority;
        }

        public override void Write()
        {
            WriteH(3842);
            if (player == null)
            {
                WriteD(0x80000000);
                return;
            }
            Clan clan = ClanManager.GetClan(player.clanId);
            WriteD(player.slotId);
            WriteS(IsAuthority ? player.nickname + $" ({player.playerId})" : player.nickname, 33);
            WriteD(player.exp);
            WriteD(player.GetRank());
            WriteD(player.rankId);
            WriteD(player.gold);
            WriteD(player.cash);
            WriteD(clan.id);
            WriteD((int)player.clanAuthority);
            WriteD(0);
            WriteD(0);
            WriteC(player.pccafe);
            WriteC(player.tourneyLevel);
            WriteC(player.nickcolor);
            WriteS(clan.name, 17);
            WriteC(clan.rank);
            WriteC(clan.GetClanUnit());
            WriteD(clan.logo);
            WriteC(clan.nameColor);
            WriteC(0);
            WriteD(0);
            WriteD(0);
            WriteD(player.lastRankUpDate);
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
            WriteD(player.equipments.red);
            WriteD(player.equipments.blue);
            WriteD(player.equipments.helmet);
            WriteD(player.equipments.beret);
            WriteD(player.equipments.dino);
            WriteD(player.equipments.primary);
            WriteD(player.equipments.secondary);
            WriteD(player.equipments.melee);
            WriteD(player.equipments.grenade);
            WriteD(player.equipments.special);
            WriteD(player.titles.Equiped1);
            WriteD(player.titles.Equiped2);
            WriteD(player.titles.Equiped3);
        }
    }
}