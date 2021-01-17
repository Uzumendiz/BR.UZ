using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_ACCEPT_BATTLE_REQ : GamePacketReader
    {
        private int id, serverInfo, type;
        public override void ReadImplement()
        {
            ReadInt();
            id = ReadShort();
            serverInfo = ReadShort();
            type = ReadByte();
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
                Match mt = player.match;
                int channelId = serverInfo - (serverInfo / 10 * 10);
                Match mt2 = ServersManager.GetChannel(channelId).GetMatch(id);
                if (mt != null && mt2 != null && player.matchSlot == mt.leader)
                {
                    if (type == 1)
                    {
                        if (mt.formação != mt2.formação)
                        {
                            client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001092_PAK);
                        }
                        else if (mt2.GetCountPlayers() != mt.formação || mt.GetCountPlayers() != mt.formação)
                        {
                            client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001091_PAK);
                        }
                        else if (mt2.state == MatchStateEnum.Play || mt.state == MatchStateEnum.Play)
                        {
                            client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001090_PAK);
                        }
                        else
                        {
                            mt.state = MatchStateEnum.Play;
                            Account pM = mt2.GetLeader();
                            if (pM != null && pM.match != null)
                            {
                                pM.SendPacket(new CLAN_WAR_ENEMY_INFO_PAK(mt));
                                pM.SendPacket(new CLAN_WAR_CREATED_ROOM_PAK(mt));
                                mt2.slots[pM.matchSlot].state = SlotMatchStateEnum.Ready;
                            }
                            mt2.state = MatchStateEnum.Play;
                            client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_SUCCESS_PAK);
                        }
                    }
                    else
                    {
                        Account pM = mt2.GetLeader();
                        if (pM != null && pM.match != null)
                        {
                            pM.SendCompletePacket(PackageDataManager.CLAN_WAR_RECUSED_BATTLE_ERROR_0x80001093_PAK);
                        }
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001094_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}