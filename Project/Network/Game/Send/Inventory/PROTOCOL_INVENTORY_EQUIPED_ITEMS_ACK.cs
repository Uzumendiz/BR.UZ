namespace PointBlank.Game
{
    public class INVENTORY_EQUIPED_ITEMS_PAK : GamePacketWriter
    {
        private InventoryFlag type;
        private PlayerEquipedItems equip;
        /// <summary>
        /// Gera um pacote que faz uma checagem em todos os itens equipados, comparando-os com o inventário.
        /// </summary>
        /// <param name="player">Conta</param>
        public INVENTORY_EQUIPED_ITEMS_PAK(Account player)
        {
            type = (InventoryFlag)player.CheckEquipedItems(player.equipments);
            equip = player.equipments;
        }
        public INVENTORY_EQUIPED_ITEMS_PAK(Account player, int type)
        {
            this.type = (InventoryFlag)type;
            equip = player.equipments;
        }

        public override void Write()
        {
            WriteH(2058);
            WriteD((int)type);
            if (type.HasFlag(InventoryFlag.Character))
            {
                WriteD(equip.red);
                WriteD(equip.blue);
                WriteD(equip.helmet);
                WriteD(equip.beret);
                WriteD(equip.dino);
            }
            if (type.HasFlag(InventoryFlag.Weapon))
            {
                WriteD(equip.primary);
                WriteD(equip.secondary);
                WriteD(equip.melee);
                WriteD(equip.grenade);
                WriteD(equip.special);
            }
        }
    }
}