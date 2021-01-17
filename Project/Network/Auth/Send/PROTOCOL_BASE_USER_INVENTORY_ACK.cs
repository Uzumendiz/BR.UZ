using System.Collections.Generic;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_INVENTORY_ACK : GamePacketWriter
    {
        private List<ItemsModel> charas = new List<ItemsModel>();
        private List<ItemsModel> weapons = new List<ItemsModel>();
        private List<ItemsModel> cupons = new List<ItemsModel>();
        public PROTOCOL_BASE_USER_INVENTORY_ACK(List<ItemsModel> items)
        {
            InventoryLoad(items);
        }
        private void InventoryLoad(List<ItemsModel> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemsModel item = items[i];
                if (item.category == 1)
                {
                    weapons.Add(item);
                }
                else if (item.category == 2)
                {
                    charas.Add(item);
                }
                else if (item.category == 3)
                {
                    cupons.Add(item);
                }
            }
        }
        public override void Write()
        {
            WriteH(2699);
            WriteD(charas.Count);
            for (int i = 0; i < charas.Count; i++)
            {
                ItemsModel item = charas[i];
                WriteQ(item.objectId);
                WriteD(item.id);
                WriteC(item.equip);
                WriteD(item.count);
            }
            WriteD(weapons.Count);
            for (int i = 0; i < weapons.Count; i++)
            {
                ItemsModel item = weapons[i];
                WriteQ(item.objectId);
                WriteD(item.id);
                WriteC(item.equip);
                WriteD(item.count);
            }
            WriteD(cupons.Count);
            for (int i = 0; i < cupons.Count; i++)
            {
                ItemsModel item = cupons[i];
                WriteQ(item.objectId);
                WriteD(item.id);
                WriteC(item.equip);
                WriteD(item.count);
            }
            WriteD(0);
            charas = null;
            weapons = null;
            cupons = null;
        }
    }
}