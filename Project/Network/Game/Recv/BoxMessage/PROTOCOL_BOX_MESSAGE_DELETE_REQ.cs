using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_BOX_MESSAGE_DELETE_REQ : GamePacketReader
    {
        private List<object> objects;
        public override void ReadImplement()
        {
            objects = new List<object>();
            int count = ReadByte();
            for (int i = 0; i < count; i++)
            {
                objects.Add(ReadInt());
            }
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null)
                {
                    return;
                }
                if (player.DeleteMessages(objects))
                {
                    client.SendPacket(new BOX_MESSAGE_DELETE_PAK(objects, 0));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_DELETE_ERROR_PAK);
                }
                objects = null;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}