using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_LIST_REQ : GamePacketReader
    {
        private int page;
        public override void ReadImplement()
        {
            page = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null)
                {
                    return;
                }
                byte count = 0;
                using (PacketWriter writer = new PacketWriter())
                {
                    lock (ClanManager.clans)
                    {
                        for (int i = page * 170; i < ClanManager.clans.Count; i++)
                        {
                            Clan clan = ClanManager.clans[i];
                            if (player.clanId != clan.id)
                            {
                                writer.WriteD(clan.id);
                                writer.WriteS(clan.name, 17);
                                writer.WriteC(clan.rank);
                                writer.WriteC(clan.GetClanPlayers());
                                writer.WriteC((byte)clan.maxPlayers);
                                writer.WriteD(clan.creationDate);
                                writer.WriteD(clan.logo);
                                writer.WriteC(clan.nameColor);
                                if (count++ == 170)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    client.SendPacket(new PROTOCOL_CLAN_LIST_ACK(page, count, writer.memorystream.ToArray()));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}