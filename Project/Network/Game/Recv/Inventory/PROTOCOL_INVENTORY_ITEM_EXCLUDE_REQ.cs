using System;

namespace PointBlank.Game
{
    public class PROTOCOL_INVENTORY_ITEM_EXCLUDE_REQ : GamePacketReader
    {
        private long objectId;
        public override void ReadImplement()
        {
            objectId = ReadLong();
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
                ItemsModel item = player.inventory.GetItem(objectId);
                if (item == null)
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EXCLUDE_0x80000000_PAK);
                    return;
                }
                else if (Utilities.GetIdStatics(item.id, 1) == 12)
                {
                    PlayerBonus bonus = player.bonus;
                    if (bonus == null)
                    {
                        client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EXCLUDE_0x80000000_PAK);
                        return;
                    }
                    bool changed = bonus.RemoveBonuses(item.id);
                    if (!changed)
                    {
                        if (item.id == 1200014000)
                        {
                            if (player.ExecuteQuery($"UPDATE player_bonus SET sightcolor='4' WHERE player_id='{player.playerId}'"))
                            {
                                bonus.sightColor = 4;
                                client.SendPacket(new PROTOCOL_BASE_USER_EFFECTS_ACK(0, bonus));
                            }
                            else
                            {
                                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EXCLUDE_0x80000000_PAK);
                                return;
                            }
                        }
                        else if (item.id == 1200010000)
                        {
                            if (bonus.fakeNick.Length == 0)
                            {
                                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EXCLUDE_0x80000000_PAK);
                                return;
                            }
                            if (player.ExecuteQuery($"UPDATE accounts SET nickname='{bonus.fakeNick}' WHERE id='{player.playerId}'") &&
                                player.ExecuteQuery($"UPDATE player_bonus SET fakenick='{""}' WHERE player_id='{player.playerId}'"))
                            {
                                player.nickname = bonus.fakeNick;
                                bonus.fakeNick = "";
                                client.SendPacket(new PROTOCOL_BASE_USER_EFFECTS_ACK(0, bonus));
                                client.SendPacket(new PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(player.nickname));
                            }
                            else
                            {
                                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EXCLUDE_0x80000000_PAK);
                                return;
                            }
                        }
                        else if (item.id == 1200009000)
                        {
                            if (player.ExecuteQuery($"UPDATE player_bonus SET fakerank='55' WHERE player_id='{player.playerId}'"))
                            {
                                bonus.fakeRank = 55;
                                client.SendPacket(new PROTOCOL_BASE_USER_EFFECTS_ACK(0, bonus));
                            }
                            else
                            {
                                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EXCLUDE_0x80000000_PAK);
                                return;
                            }
                        }
                        else if (item.id == 1200006000)
                        {
                            if (player.ExecuteQuery($"UPDATE accounts SET nickcolor='0' WHERE id='{player.playerId}'"))
                            {
                                player.nickcolor = 0;
                                client.SendPacket(new BASE_2612_PAK(player));
                                Room room = player.room;
                                if (room != null)
                                {
                                    using (PROTOCOL_ROOM_GET_NICKNAME_ACK packet = new PROTOCOL_ROOM_GET_NICKNAME_ACK(player.slotId, player.nickname, player.nickcolor))
                                    {
                                        room.SendPacketToPlayers(packet);
                                    }
                                }
                            }
                            else
                            {
                                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EXCLUDE_0x80000000_PAK);
                                return;
                            }
                        }
                    }
                    else
                    {
                        player.UpdatePlayerBonus();
                    }
                }
                if (item != null)
                {
                    if (player.DeleteItem(item.objectId))
                    {
                        player.inventory.RemoveItem(item);
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EXCLUDE_0x80000000_PAK);
                        return;
                    }
                }
                client.SendPacket(new INVENTORY_ITEM_EXCLUDE_PAK(1, objectId));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}