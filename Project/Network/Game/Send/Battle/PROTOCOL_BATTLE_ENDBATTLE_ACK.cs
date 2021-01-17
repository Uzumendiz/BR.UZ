namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_ENDBATTLE_ACK : GamePacketWriter
    {
        private Room room;
        private Account player;
        private TeamResultTypeEnum winner = TeamResultTypeEnum.TeamDraw;
        private ushort playersFlag, missionsFlag;
        private bool isBotMode;
        private byte[] array1;
        public PROTOCOL_BATTLE_ENDBATTLE_ACK(Account player)
        {
            this.player = player;
            if (player != null)
            {
                room = player.room;
                winner = room.GetWinnerTeam();
                isBotMode = room.IsBotMode();
                room.GetBattleResult(out missionsFlag, out playersFlag, out array1);
            }
        }
        public PROTOCOL_BATTLE_ENDBATTLE_ACK(Account player, TeamResultTypeEnum winner, ushort playersFlag, ushort missionsFlag, bool isBotMode, byte[] a1)
        {
            this.player = player;
            this.winner = winner;
            this.playersFlag = playersFlag;
            this.missionsFlag = missionsFlag;
            this.isBotMode = isBotMode;
            array1 = a1;
            if (player != null)
            {
                room = player.room;
            }
        }
        public override void Write()
        {
            if (room == null)
            {
                return;
            }
            WriteH(3336);
            WriteC((byte)winner);
            WriteH(playersFlag);
            WriteH(missionsFlag);
            WriteB(array1);
            Clan clan = ClanManager.GetClan(player.clanId);
            WriteS(player.nickname, 33);
            WriteD(player.exp);
            WriteD(player.rankId);
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
            WriteD(0);
            WriteC(0);
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
            if (isBotMode)
            {
                for (int i = 0; i < 16; i++)
                {
                    WriteH(room.slots[i].score);
                }
            }
            else if (room.mode == RoomTypeEnum.Destruction || room.mode == RoomTypeEnum.Suppression)
            {
                WriteH(room.redRounds);
                WriteH(room.blueRounds);
                for (int i = 0; i < 16; i++)
                {
                    WriteC((byte)room.slots[i].objetivos);
                }
            }
            else if (room.mode == RoomTypeEnum.Dino)
            {
                WriteH(room.redDino);
                WriteH(room.blueDino);
                for (int i = 0; i < 16; i++)
                {
                    WriteC((byte)room.slots[i].objetivos);
                }
            }
            else if (room.mode == RoomTypeEnum.CrossCounter)
            {
                WriteH(room.redKills);
                WriteH(room.blueKills);
                for (int i = 0; i < 16; i++)
                {
                    WriteC((byte)room.slots[i].objetivos);
                }
            }
            WriteC(0);
            WriteD(0);
            WriteB(new byte[16]);
        }
    }
}