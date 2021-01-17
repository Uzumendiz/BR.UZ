using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_LIST_CONTEXT_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player != null)
                {
                    client.SendPacket(new PROTOCOL_CLAN_LIST_CONTEXT_ACK(ClanManager.clans.Count));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}