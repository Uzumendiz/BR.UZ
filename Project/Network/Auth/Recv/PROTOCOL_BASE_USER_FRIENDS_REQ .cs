using System;
using System.Collections.Generic;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_FRIENDS_REQ : AuthPacketReader
    {
        public override void ReadImplement()
        {         
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.checkUserFriends)
                {
                    return;
                }
                player.checkUserFriends = true;
                if (!player.checkSourceInfo || !player.checkUserInfo)
                {
                    Logger.Attacks($" [Auth] (PROTOCOL_BASE_USER_FRIENDS_REQ) Connection destroyed on suspicion of modified client. IPAddress: {client.GetIPAddress()}");
                    client.Close(0, true);
                    return;
                }
                FirewallSecurity.AddRuleTcp(client.GetIPAddress());
                client.SendPacket(new PROTOCOL_BASE_EXIT_URL_ACK(Settings.ExitUrl));
                player.LoadPlayerFriends(false);
                player.UpdateCommunity(true);
                List<Friend> friends = player.friends.friendsCache;
                if (friends.Count > 0)
                {
                    client.SendPacket(new PROTOCOL_BASE_USER_FRIENDS_ACK(friends));
                }
                List<Message> messages = player.GetMessages();
                if (messages.Count > 0)
                {
                    player.RecicleMessages(messages);
                    if (messages.Count > 0)
                    {
                        byte pages = (byte)Math.Ceiling(messages.Count / 25d);
                        for (byte i = 0; i < pages; i++)
                        {
                            client.SendPacket(new PROTOCOL_BASE_USER_MESSAGES_ACK(i, messages));
                        }
                    }
                }
                client.SendPacket(new PROTOCOL_BASE_USER_CONFIG_ACK(0, player.configs));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}