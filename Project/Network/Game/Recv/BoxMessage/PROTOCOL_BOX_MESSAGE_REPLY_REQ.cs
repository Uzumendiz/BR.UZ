using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BOX_MESSAGE_REPLY_REQ : GamePacketReader
    {
        private long receiverId;
        private string text;
        public override void ReadImplement()
        {
            receiverId = ReadLong();
            text = ReadString(ReadByte());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (text.Length > 120 || player == null || player.playerId == receiverId)
                {
                    return;
                }
                Account playerReceive = AccountManager.GetAccount(receiverId, 0); //Player que recebe a resposta da mensagem.
                if (playerReceive != null)
                {
                    if (playerReceive.GetMessagesCount() >= 100)
                    {
                        client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_CREATE_ERROR_0x8000107F_PAK);
                        return;
                    }
                    Message message = new Message(15)
                    {
                        senderName = player.nickname,
                        senderId = player.playerId,
                        text = text,
                        state = 1
                    };
                    if (message != null && playerReceive.InsertMessage(message))
                    {
                        if (playerReceive.isOnline)
                        {
                            playerReceive.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                        }
                        client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_CREATE_SUCCESS_PAK);
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_CREATE_ERROR_0x80000000_PAK);
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_CREATE_ERROR_0x8000107E_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}