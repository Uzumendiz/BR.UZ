using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_BOX_MESSAGE_VIEW_REQ : GamePacketReader
    {
        private int messagesCount;
        private List<int> messages = new List<int>();
        public override void ReadImplement()
        {
            messagesCount = ReadByte();
            for (int i = 0; i < messagesCount; i++)
            {
                messages.Add(ReadInt());
            }
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || messagesCount == 0)
                {
                    return;
                }
                player.UpdateMessage(messages.ToArray(), int.Parse(DateTime.Now.AddDays(7).ToString("yyMMddHHmm")));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
            finally
            {
                messages = null;
            }
        }
    }
}