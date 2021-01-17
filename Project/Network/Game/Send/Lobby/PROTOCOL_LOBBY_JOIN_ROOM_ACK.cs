using System.Collections.Generic;

namespace PointBlank.Game
{
    public class LOBBY_JOIN_ROOM_PAK : GamePacketWriter
    {
        private uint error;
        private Room room;
        private int slotId;
        private Account leader;
        public LOBBY_JOIN_ROOM_PAK(uint error, Account player = null, Account leader = null)
        {
            this.error = error;
            if (player != null)
            {
                slotId = player.slotId;
                room = player.room;
                this.leader = leader;
            }
        }
        public override void Write()
        {
            WriteH(3082);
            if (error == 0 && room != null && leader != null)
            {
                lock (room.slots)
                {
                    WriteData();
                }
            }
            else
            {
                WriteD(error);
            }
        }
        private void WriteData()
        {
            List<Account> roomPlayers = room.GetAllPlayers();
            WriteD(room.roomId);
            WriteD(slotId);
            WriteD(room.roomId);
            WriteS(room.roomName, 23);
            WriteH((ushort)room.mapId);
            WriteC(room.stage4vs4);
            WriteC((byte)room.mode);
            WriteC((byte)room.state);
            WriteC((byte)roomPlayers.Count);
            WriteC(room.GetSlotCount());
            WriteC(room.ping);
            WriteC(room.weaponsFlag);
            WriteC(room.randomMap);
            WriteC((byte)room.modeSpecial);
            WriteS(leader.nickname, 33);
            WriteD(room.killtime);
            WriteC(room.limit);
            WriteC(room.seeConf);
            WriteH((ushort)room.balancing);
            WriteS(room.password, 4);
            WriteC((byte)room.countdown.GetTimeLeft());
            WriteD(room.leaderSlot);
            for (int i = 0; i < 16; ++i)
            {
                Slot slot = room.slots[i];
                Account player = room.GetPlayerBySlot(slot);
                if (player != null)
                {
                    Clan clan = ClanManager.GetClan(player.clanId);
                    WriteC((byte)slot.state);
                    WriteC((byte)player.GetRank());
                    WriteD(clan.id);
                    WriteD((int)player.clanAuthority);
                    WriteC(clan.rank);
                    WriteD(clan.logo);
                    WriteC(player.pccafe);
                    WriteC(player.tourneyLevel);
                    WriteD((uint)player.effects);
                    WriteS(clan.name, 17);
                    WriteD(0);
                    WriteC(player.country);
                }
                else
                {
                    WriteC((byte)slot.state);
                    WriteB(new byte[10]);
                    WriteD(4294967295);
                    WriteB(new byte[28]);
                }
            }
            WriteC((byte)roomPlayers.Count);
            for (int i = 0; i < roomPlayers.Count; i++)
            {
                Account ac = roomPlayers[i];
                WriteC((byte)ac.slotId);
                WriteC((byte)(ac.nickname.Length + 1));
                WriteS(ac.nickname, ac.nickname.Length + 1);
                WriteC(ac.nickcolor);
            }
            if (room.IsBotMode())
            {
                WriteC(room.aiCount);
                WriteC(room.aiLevel);
            }
        }
    }
}