using System;

namespace PointBlank.Game
{
    public class PROTOCOL_SHOP_ENTER_REQ : GamePacketReader
    {
        private int Unknown;
        public override void ReadImplement()
        {
            Unknown = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                DateTime now = DateTime.Now;
                if (player == null || !player.loadedShop || (now - player.lastShopEnter).TotalSeconds < 1)
                {
                    return;
                }
                if (Unknown != 44)
                {
                    //Unknown ficou 0 quando loguei no jogo pela primeira vez, no radmin, pela vps nao tinha chamado o valor 0. e o shop tava vazio na db
                    Logger.Warning(" [PROTOCOL_SHOP_ENTER_REQ] Unk: " + Unknown + " PlayerId: " + player.playerId);
                    //return;
                }
                Room room = player.room;
                if (room != null)
                {
                    room.ChangeSlotState(player.slotId, SlotStateEnum.SHOP, false);
                    room.StopCountDown(player.slotId);
                    room.UpdateSlotsInfo();
                }
                client.SendPacket(new SHOP_ENTER_PAK());
                player.lastShopEnter = now;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}