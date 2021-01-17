using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_SEND_WHISPER_REQ : GamePacketReader
    {
        private string receiverName, text;
        public override void ReadImplement()
        {
            receiverName = ReadString(33);
            text = ReadString(ReadShort());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                DateTime now = DateTime.Now;
                if (player == null || receiverName.Length == 0 || text.Length == 0 || player.nickname == receiverName || (now - player.lastChatting).TotalSeconds < 1)
                {
                    return;
                }
                Account playerReceive = AccountManager.GetAccountByNick(receiverName);
                if (playerReceive == null || playerReceive.nickname != receiverName || !playerReceive.isOnline)
                {
                    client.SendPacket(new AUTH_SEND_WHISPER_PAK(receiverName, text, 0x80000000));
                }
                else
                {
                    playerReceive.SendPacket(new AUTH_RECV_WHISPER_PAK(player.nickname, text, player.UseChatGM()));
                }
                player.lastChatting = now;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}