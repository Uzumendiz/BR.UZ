using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_SEND_WHISPER2_REQ : GamePacketReader
    {
        private long receiverId;
        private string receiverName, text;
        public override void ReadImplement()
        {
            receiverId = ReadLong();
            receiverName = ReadString(33);
            text = ReadString(ReadShort());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                DateTime now = DateTime.Now;
                if (player == null || receiverId <= 0 || receiverName.Length == 0 || text.Length == 0 || player.playerId == receiverId || player.nickname == receiverName || (now - player.lastChatting).TotalSeconds < 1)
                {
                    return;
                }
                Account playerReceive = AccountManager.GetAccount(receiverId, true);
                Logger.Warning($" WHI2: Isnull: {(playerReceive == null)} Is online: {(playerReceive != null ? playerReceive.isOnline : false)} CheckNick: {(playerReceive != null ? playerReceive.nickname != receiverName : false)}");
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