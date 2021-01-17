using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_INVENTORY_ITEM_CREATE_ACK : GamePacketWriter
    {
        private Account player;
        private byte type;
        private List<ItemsModel> weapons = new List<ItemsModel>(),
                                 charas = new List<ItemsModel>(),
                                 cupons = new List<ItemsModel>();
        public PROTOCOL_INVENTORY_ITEM_CREATE_ACK(byte type, Account player, List<ItemsModel> items)
        {
            this.type = type;
            this.player = player;
            AddItems(items);
        }
        public PROTOCOL_INVENTORY_ITEM_CREATE_ACK(byte type, Account player, ItemsModel item)
        {
            this.type = type;
            this.player = player;
            AddItems(item);
        }
        public override void Write()
        {
            WriteH(3588);
            WriteC(type);
            WriteD(charas.Count);
            WriteD(weapons.Count);
            WriteD(cupons.Count);
            for (int i = 0; i < charas.Count; i++)
            {
                ItemsModel item = charas[i];
                WriteQ(item.objectId);
                WriteD(item.id);
                WriteC(item.equip);
                WriteD(item.count);
            }
            for (int i = 0; i < weapons.Count; i++)
            {
                ItemsModel item = weapons[i];
                WriteQ(item.objectId);
                WriteD(item.id);
                WriteC(item.equip);
                WriteD(item.count);
            }
            for (int i = 0; i < cupons.Count; i++)
            {
                ItemsModel item = cupons[i];
                WriteQ(item.objectId);
                WriteD(item.id);
                WriteC(item.equip);
                WriteD(item.count);
            }
        }

        private void AddItems(List<ItemsModel> items)
        {
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ItemsModel item = items[i];
                    ItemsModel modelo = new ItemsModel(item) { objectId = item.objectId };
                    if (type == 1)
                    {
                        player.TryCreateItem(modelo);
                    }
                    ItemsModel ItemExist = player.inventory.GetItem(item.objectId);
                    if (ItemExist == null)
                    {
                        player.inventory.AddItem(new ItemsModel { objectId = item.objectId, id = item.id, equip = item.equip, count = item.count, category = item.category, name = "" });
                    }
                    else
                    {
                        ItemExist.count = item.count;
                    }
                    if (modelo.category == 1)
                    {
                        weapons.Add(modelo);
                    }
                    else if (modelo.category == 2)
                    {
                        charas.Add(modelo);
                    }
                    else if (modelo.category == 3)
                    {
                        cupons.Add(modelo);
                    }
                }
            }
            catch (Exception ex)
            {
                player.Close();
                Logger.Exception(ex);
            }
        }

        private void AddItems(ItemsModel item)
        {
            try
            {
                ItemsModel modelo = new ItemsModel(item) { objectId = item.objectId };
                if (type == 1)
                {
                    player.TryCreateItem(modelo);
                }
                ItemsModel ItemExist = player.inventory.GetItem(item.objectId);
                if (ItemExist == null)
                {
                    player.inventory.AddItem(new ItemsModel { objectId = item.objectId, id = item.id, equip = item.equip, count = item.count, category = item.category, name = "" });
                }
                else
                {
                    ItemExist.count = item.count;
                }
                if (modelo.category == 1)
                {
                    weapons.Add(modelo);
                }
                else if (modelo.category == 2)
                {
                    charas.Add(modelo);
                }
                else if (modelo.category == 3)
                {
                    cupons.Add(modelo);
                }
            }
            catch (Exception ex)
            {
                player.Close();
                Logger.Exception(ex);
            }
        }
    }
}