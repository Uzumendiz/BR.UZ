using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CREATE_REQUIREMENTS_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.clanId > 0)
                {
                    return;
                }
                client.SendCompletePacket(PackageDataManager.CLAN_CREATE_REQUIREMENTS_PAK);
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}