using System.Collections.Generic;

namespace PointBlank
{
    public class PlayerInventory
    {
        public List<ItemsModel> items = new List<ItemsModel>();
        public ItemsModel GetItem(int id)
        {
            lock (items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ItemsModel item = items[i];
                    if (item.id == id)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public ItemsModel GetItem(long obj)
        {
            lock (items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ItemsModel item = items[i];
                    if (item.objectId == obj)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public void LoadBasicItems()
        {
            lock (items)
            {
                items.AddRange(DefaultInventoryManager.defaults);
            }
        }

        public List<ItemsModel> GetItemsByType(int type)
        {
            List<ItemsModel> list = new List<ItemsModel>();
            lock (items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ItemsModel item = items[i];
                    if (item.category == type || item.id > 1200000000 && item.id < 1300000000 && type == 4)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Remove um item específico do inventário do jogador através do número do objeto. (Proteção de Thread-Safety)
        /// </summary>
        /// <param name="obj">Número do objeto</param>
        public void RemoveItem(long obj)
        {
            lock (items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ItemsModel item = items[i];
                    if (item.objectId == obj)
                    {
                        items.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Remove um item específico do inventário do jogador através do item. (Proteção de Thread-Safety)
        /// </summary>
        /// <param name="item">Modelo do item</param>
        public bool RemoveItem(ItemsModel item)
        {
            lock (items)
            {
                return items.Remove(item);
            }
        }

        /// <summary>
        /// Adiciona um item no inventário do jogador. (Proteção de Thread-Safety)
        /// </summary>
        /// <param name="item">Modelo do item</param>
        public void AddItem(ItemsModel item)
        {
            lock (items)
            {
                items.Add(item);
            }
        }
    }
}