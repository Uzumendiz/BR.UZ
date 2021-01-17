using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_INVENTORY_ITEM_EFFECT_REQ : GamePacketReader
    {
        private long objectId;
        private uint objetivo;
        private byte[] info;
        private string txt;
        public override void ReadImplement()
        {
            objectId = ReadLong();
            info = ReadB(ReadByte());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                ItemsModel item = player != null ? player.inventory.GetItem(objectId) : null;
                if (item != null && item.id > 1300000000)
                {
                    int cuponId = CreateItemId(12, item.id % 100000000 / 1000000, item.id % 1000000 / 1000, 0);
                    int cuponDays = int.Parse(DateTime.Now.AddDays(item.id % 1000).ToString("yyMMddHHmm"));
                    if (cuponId == 1201047000 || cuponId == 1201051000 || cuponId == 1200010000)
                    {
                        txt = ArrayToString(info, info.Length);
                    }
                    else if (cuponId == 1201052000 || cuponId == 1200005000)
                    {
                        objetivo = BitConverter.ToUInt32(info, 0);
                    }
                    else if (info.Length > 0)
                    {
                        objetivo = info[0];
                    }
                    CreateCuponEffects(cuponId, cuponDays, player);
                    client.SendPacket(new INVENTORY_ITEM_EQUIP_PAK(1, item, player));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                }
            }
            catch (Exception ex)
            {
                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                PacketLog(ex);
            }
        }

        /// <summary>
        /// Gera efeitos dos cupons na Database.
        /// </summary>
        /// <param name="cuponId">Id do cupom</param>
        /// <param name="cuponDays">Dias do cupom</param>
        /// <param name="player">Jogador</param>
        private void CreateCuponEffects(int cupomId, int cuponDays, Account player)
        {
            if (cupomId == 1201051000)
            {
                Clan clan = ClanManager.GetClan(player.clanId);
                if (clan.id > 0 && clan.ownerId == player.playerId)
                {
                    if (!ClanManager.CheckNameLengthInvalid(txt) && StringFilter.CheckStringFilter(txt) && !ClanManager.IsClanNameExist(txt).Result && clan.UpdateName(txt).Result)
                    {
                        clan.name = txt;
                        List<Account> players = player.GetClanPlayers(-1);
                        using (CLAN_CHANGE_NAME_PAK packet = new CLAN_CHANGE_NAME_PAK(txt))
                        {
                            player.SendPacketForPlayers(packet, players);
                        }
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                        return;
                    }
                }
            }
            else if (cupomId == 1201052000)
            {
                Clan clan = ClanManager.GetClan(player.clanId);
                if (clan.id > 0 && clan.ownerId == player.playerId && !ClanManager.IsClanLogoExist(objetivo).Result && clan.UpdateLogo(objetivo).Result)
                {
                    clan.logo = objetivo;
                    List<Account> players = player.GetClanPlayers(-1);
                    using (CLAN_CHANGE_LOGO_PAK packet = new CLAN_CHANGE_LOGO_PAK(objetivo))
                    {
                        player.SendPacketForPlayers(packet, players);
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                    return;
                }
            }
            else if (cupomId == 1201047000)
            {
                if (!AccountManager.CheckNickLengthInvalid(txt) && StringFilter.CheckStringFilter(txt) && !AccountManager.CheckNicknameExist(txt).Result && player.UpdateNick(txt).Result)
                {
                    player.nickname = txt;
                    if (!NickHistoryManager.CreateHistory(player.playerId, player.nickname, txt, "Change nick"))
                    {
                        Logger.Warning($" [GAME] [PROTOCOL_INVENTORY_ITEM_EFFECT_REQ] (CuponId: 1201047000) Não foi possivel salvar o histórico de nome. PlayerId: {player.playerId} Nickname: {txt} Motivo: Change nick.");
                    }
                    if (player.room != null)
                    {
                        using (PROTOCOL_ROOM_GET_NICKNAME_ACK packet = new PROTOCOL_ROOM_GET_NICKNAME_ACK(player.slotId, player.nickname, player.nickcolor))
                        {
                            player.room.SendPacketToPlayers(packet);
                        }
                    }
                    client.SendPacket(new PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(player.nickname));
                    if (player.clanId > 0)
                    {
                        List<Account> players = player.GetClanPlayers(-1);
                        using (PROTOCOL_CLAN_MEMBER_INFO_UPDATE_ACK packet = new PROTOCOL_CLAN_MEMBER_INFO_UPDATE_ACK(player))
                        {
                            player.SendPacketForPlayers(packet, players);
                        }
                    }
                    player.SyncPlayerToFriends(true);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_2147483923_PAK);
                    return;
                }
            }
            else if (cupomId == 1200006000)
            {
                if (player.ExecuteQuery($"UPDATE accounts SET nickcolor='{(int)objetivo}' WHERE id='{player.playerId}'"))
                {
                    player.nickcolor = (byte)objetivo;
                    client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, new ItemsModel(cupomId, 3, "NameColor [Active]", 2, cuponDays, 0)));
                    client.SendPacket(new BASE_2612_PAK(player));
                    if (player.room != null)
                    {
                        using (PROTOCOL_ROOM_GET_NICKNAME_ACK packet = new PROTOCOL_ROOM_GET_NICKNAME_ACK(player.slotId, player.nickname, player.nickcolor))
                        {
                            player.room.SendPacketToPlayers(packet);
                        }
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                    return;
                }
            }
            else if (cupomId == 1200009000)
            {
                if ((int)objetivo >= 51 || (int)objetivo < player.rankId - 10 || (int)objetivo > player.rankId + 10)
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                    return;
                }
                else if (player.ExecuteQuery($"UPDATE player_bonus SET fakerank='{(int)objetivo}' WHERE player_id='{player.playerId}'"))
                {
                    player.bonus.fakeRank = (int)objetivo;
                    client.SendPacket(new PROTOCOL_BASE_USER_EFFECTS_ACK(info.Length, player.bonus));
                    client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, new ItemsModel(cupomId, 3, "Patente falsa [Active]", 2, cuponDays, 0)));
                    if (player.room != null)
                    {
                        player.room.UpdateSlotsInfo();
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                    return;
                }
            }
            else if (cupomId == 1200010000)
            {
                if (AccountManager.CheckNickLengthInvalid(txt))
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                    return;
                }
                if (StringFilter.CheckStringFilter(txt) && !AccountManager.CheckNicknameExist(txt).Result && player.UpdateFakeNick().Result && player.UpdateNick(txt).Result)
                {
                    player.bonus.fakeNick = player.nickname;
                    player.nickname = txt;
                    if (!NickHistoryManager.CreateHistory(player.playerId, player.nickname, txt, "Change nick false"))
                    {
                        Logger.Warning($" [GAME] [PROTOCOL_INVENTORY_ITEM_EFFECT_REQ] (CuponId: 1200010000) Não foi possivel salvar o histórico de nome. PlayerId: {player.playerId} Nickname: {txt} Motivo: Change nick false.");
                    }
                    client.SendPacket(new PROTOCOL_BASE_USER_EFFECTS_ACK(info.Length, player.bonus));
                    client.SendPacket(new PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(player.nickname));
                    client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, new ItemsModel(cupomId, 3, "FakeNick [Active]", 2, cuponDays, 0)));
                    Room room = player.room;
                    if (room != null)
                    {
                        room.UpdateSlotsInfo();
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_2147483923_PAK);
                    return;
                }
            }
            else if (cupomId == 1200014000)
            {
                if (player.ExecuteQuery($"UPDATE player_bonus SET sightcolor='{(int)objetivo}' WHERE player_id='{player.playerId}'"))
                {
                    player.bonus.sightColor = (short)objetivo;
                    client.SendPacket(new PROTOCOL_BASE_USER_EFFECTS_ACK(info.Length, player.bonus));
                    client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, new ItemsModel(cupomId, 3, "Cor da mira [Active]", 2, cuponDays, 0)));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                    return;
                }
            }
            else if (cupomId == 1200005000)
            {
                Clan clan = ClanManager.GetClan(player.clanId);
                if (clan.id > 0 && clan.ownerId == player.playerId && player.ExecuteQuery($"UPDATE clan_data SET color='{(int)objetivo}' WHERE clan_id='{clan.id}'"))
                {
                    clan.nameColor = (byte)objetivo;
                    client.SendPacket(new CLAN_CHANGE_NAME_COLOR_PAK(clan.nameColor));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                    return;
                }
            }
            else if (cupomId == 1201085000)
            {
                if (player.room != null)
                {
                    Account playerRoom = player.room.GetPlayerBySlot((int)objetivo);
                    if (playerRoom != null)
                    {
                        client.SendPacket(new PROTOCOL_ROOM_INSPECTPLAYER_ACK(playerRoom));
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                        return;
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                    return;
                }
            }
            else
            {
                Logger.Error("[ITEM_EFFECT] Efeito do cupom não encontrado! Id: " + cupomId);
                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                return;
            }
        }

        public int CreateItemId(int class1, int usage, int classtype, int number)
        {
            try
            {
                return (class1 * 100000000) + (usage * 1000000) + (classtype * 1000) + number;
            }
            catch
            {
                return 0;
            }
        }

        public static string ArrayToString(byte[] buffer, int length)
        {
            string value = "";
            try
            {
                value = Settings.EncodingText.GetString(buffer, 0, length);
                int idx = value.IndexOf(char.MinValue);
                if (idx != -1)
                {
                    value = value.Substring(0, idx);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return value;
        }
    }
}