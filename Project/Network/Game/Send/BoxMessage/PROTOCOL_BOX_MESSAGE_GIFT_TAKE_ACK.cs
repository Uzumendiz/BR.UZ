using System.Collections.Generic;

namespace PointBlank.Game
{
    public class BOX_MESSAGE_GIFT_TAKE_PAK : GamePacketWriter
    {
        private List<ItemsModel> charas = new List<ItemsModel>();
        private List<ItemsModel> weapons = new List<ItemsModel>();
        private List<ItemsModel> cupons = new List<ItemsModel>();
        private uint error;
        public BOX_MESSAGE_GIFT_TAKE_PAK(uint error, ItemsModel item = null, Account p = null)
        {
            this.error = error;
            if (error == 1)
            {
                Get(item, p);
            }
        }

        public override void Write()
        {
            WriteH(541);
            WriteD(error); //2231369729 - erro | 1 - sucesso
            if (error == 1)
            {
                WriteD(charas.Count);
                WriteD(weapons.Count);
                WriteD(cupons.Count);
                WriteD(0);
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
        }

        private void Get(ItemsModel item, Account p)
        {
            try
            {
                ItemsModel modelo = new ItemsModel(item) { objectId = item.objectId };
                p.TryCreateItem(modelo);
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
            catch
            {
                p.Close();
            }
        }
    }
}