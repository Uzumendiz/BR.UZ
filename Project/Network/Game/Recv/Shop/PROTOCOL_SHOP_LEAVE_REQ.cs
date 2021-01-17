using System;

namespace PointBlank.Game
{
    public class PROTOCOL_SHOP_LEAVE_REQ : GamePacketReader
    {
        private int type;
        private PlayerEquipedItems data;
        public override void ReadImplement()
        {
            type = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                DateTime now = DateTime.Now;
                if (player == null || (now - player.lastShopLeave).TotalSeconds < 1)
                {
                    return;
                }
                data = new PlayerEquipedItems();
                using (DBQuery query = new DBQuery())
                {
                    if ((type & 1) == 1)
                    {
                        data.red = ReadInt();
                        data.blue = ReadInt();
                        data.helmet = ReadInt();
                        data.beret = ReadInt();
                        data.dino = ReadInt();
                        player.UpdateChars(data, player.equipments, query);
                    }
                    if ((type & 2) == 2)
                    {
                        data.primary = ReadInt();
                        data.secondary = ReadInt();
                        data.melee = ReadInt();
                        data.grenade = ReadInt();
                        data.special = ReadInt();
                        player.UpdateWeapons(data, player.equipments, query);
                    }
                    if (Utilities.UpdateDB("accounts", "id", player.playerId, query.GetTables(), query.GetValues()))
                    {
                        if ((type & 1) == 1)
                        {
                            player.equipments.red = data.red;
                            player.equipments.blue = data.blue;
                            player.equipments.helmet = data.helmet;
                            player.equipments.beret = data.beret;
                            player.equipments.dino = data.dino;
                        }
                        if ((type & 2) == 2)
                        {
                            player.equipments.primary = data.primary;
                            player.equipments.secondary = data.secondary;
                            player.equipments.melee = data.melee;
                            player.equipments.grenade = data.grenade;
                            player.equipments.special = data.special;
                        }
                    }
                }
                Room room = player.room;
                if (room != null)
                {
                    if (type > 0 && room.GetSlot(player.slotId, out Slot slot))
                    {
                        slot.equipment = player.equipments;
                    }
                    room.ChangeSlotState(player.slotId, SlotStateEnum.NORMAL, true);
                }
                client.SendCompletePacket(PackageDataManager.SHOP_LEAVE_PAK);
                client.SendPacket(new INVENTORY_EQUIPED_ITEMS_PAK(player));
                player.lastShopLeave = now;
                data = null;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}