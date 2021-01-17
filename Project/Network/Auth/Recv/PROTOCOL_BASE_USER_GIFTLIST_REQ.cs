using System;
using System.Collections.Generic;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_GIFTLIST_REQ : AuthPacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.checkUserGiftList || !Settings.GiftSystem)
                {
                    return;
                }
                player.checkUserGiftList = true;
                List<Message> gifts = player.GetGifts();
                if (gifts.Count > 0)
                {
                    player.RecicleMessages(gifts);
                    if (gifts.Count > 0)
                    {
                        client.SendPacket(new PROTOCOL_BASE_USER_GIFT_LIST_ACK(0, gifts));
                    }
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}