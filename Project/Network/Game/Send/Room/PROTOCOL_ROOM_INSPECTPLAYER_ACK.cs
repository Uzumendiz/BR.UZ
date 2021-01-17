using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_INSPECTPLAYER_ACK : GamePacketWriter
    {
        private Account player;
        public PROTOCOL_ROOM_INSPECTPLAYER_ACK(Account player)
        {
            this.player = player;
        }

        public override void Write()
        {
            WriteH(3893);
            WriteD(player.equipments.primary);
            WriteD(player.equipments.secondary);
            WriteD(player.equipments.melee);
            WriteD(player.equipments.grenade);
            WriteD(player.equipments.special);
            WriteD(player.equipments.red);
            WriteD(player.equipments.blue);
            WriteD(player.equipments.helmet);
            WriteD(player.equipments.beret);
            WriteD(player.equipments.dino);
            List<ItemsModel> cupons = player.inventory.GetItemsByType(4);
            WriteD(cupons.Count);
            for (int i = 0; i < cupons.Count; i++)
            {
                ItemsModel item = cupons[i];
                WriteD(item.id);
            }
        }
    }
}