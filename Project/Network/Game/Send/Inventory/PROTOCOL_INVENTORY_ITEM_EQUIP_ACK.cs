using System;

namespace PointBlank.Game
{
    public class INVENTORY_ITEM_EQUIP_PAK : GamePacketWriter
    {
        private ItemsModel item;
        private uint error;
        public INVENTORY_ITEM_EQUIP_PAK(uint error, ItemsModel item = null, Account player = null)
        {
            this.error = error;
            if (error != 1)
            {
                return;
            }
            if (item != null)
            {
                int WeaponClass = Utilities.GetIdStatics(item.id, 1);
                if (WeaponClass == 13 || WeaponClass == 15)
                {
                    if (item.count > 1 && item.equip == 1)
                    {
                        player.ExecuteQuery($"UPDATE player_items SET count='{item.count--}' WHERE owner_id='{player.playerId}' AND object_id='{item.objectId}'");
                    }
                    else
                    {
                        player.DeleteItem(item.objectId);
                        player.inventory.RemoveItem(item);
                        item.id = 0;
                        item.count = 0;
                    }
                }
                else
                {
                    item.equip = 2;
                }
                this.item = item;
            }
            else
            {
                this.error = 0x80000000;
            }
        }

        public override void Write()
        {
            WriteH(535);
            WriteD(error);
            if (error == 1)
            {
                WriteD(int.Parse(DateTime.Now.ToString("yyMMddHHmm")));
                WriteQ(item.objectId);
                WriteD(item.id);
                WriteC(item.equip);
                WriteD(item.count);
            }
            //0x80001086 STR_TBL_GUI_BASE_NO_EQUIP_PRE_DESIGNATION
        }
    }
}