namespace PointBlank
{
    public class ItemsModel
    {
        public int id;
        public int category;
        public byte equip;
        public string name;
        public long objectId;
        public int count;
        public ItemsModel DeepCopy() => (ItemsModel)MemberwiseClone();
        public ItemsModel() { }
        /// <summary>
        /// 'category' definido automaticamente.
        /// </summary>
        /// <param name="id"></param>
        public ItemsModel(int id)
        {
            SetItemId(id);
        }
        /// <summary>
        /// 'category' definido automaticamente.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="equip"></param>
        /// <param name="count"></param>
        /// <param name="objId"></param>
        public ItemsModel(int id, string name, byte equip, int count, long objectId = 0)
        {
            this.objectId = objectId;
            SetItemId(id);
            this.name = name;
            this.equip = equip;
            this.count = count;
        }
        public ItemsModel(int id, int category, string name, byte equip, int count, long objectId = 0)
        {
            this.objectId = objectId;
            this.id = id;
            this.category = category;
            this.name = name;
            this.equip = equip;
            this.count = count;
        }

        /// <summary>
        /// Faz uma cópia de outro Modelo.
        /// <para>Não faz cópia do 'objId'</para>
        /// </summary>
        /// <param name="item"></param>
        public ItemsModel(ItemsModel item)
        {
            id = item.id;
            category = item.category;
            name = item.name;
            count = item.count;
            equip = item.equip;
        }

        public void SetItemId(int id)
        {
            this.id = id;
            category = Utilities.GetItemCategory(id);
        }
    }
}