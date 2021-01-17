using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BOX_MESSAGE_CREATE_REQ : GamePacketReader
    {
        private byte nicknameLength;
        private byte textLength;
        private string nickname;
        private string text;
        public override void ReadImplement()
        {
            nicknameLength = ReadByte();
            textLength = ReadByte();
            nickname = ReadString(nicknameLength);
            text = ReadString(textLength);
        }

        public override void RunImplement()
        {
            try
            {
                //0x80001080 STR_TBL_NETWORK_DONT_SEND_MYSELF_MESSAGE
                //0x80001081 STR_TBL_NETWORK_FULL_SEND_MESSAGE_PER_DAY
                Account player = client.SessionPlayer;
                if (text.Length > 120 || player == null || player.nickname.Length == 0 || player.nickname == nickname)
                {
                    return;
                }
                Account playerReceive = AccountManager.GetAccount(nickname, 0); //Player que recebe a mensagem
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