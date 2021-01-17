using System;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_INVENTORY_REQ : AuthPacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player != null && !player.checkUserInventory)
                {
                    player.checkUserInventory = true;
                    client.SendPacket(new PROTOCOL_BASE_USER_INVENTORY_ACK(player.inventory.items));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}