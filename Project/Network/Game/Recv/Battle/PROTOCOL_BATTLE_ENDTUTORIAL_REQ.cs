using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_ENDTUTORIAL_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                if (client.SessionPlayer == null)
                {
                    return;
                }
                client.SendCompletePacket(PackageDataManager.BATTLE_TUTORIAL_ROUND_END_PAK);
                client.SendPacket(new PROTOCOL_BATTLE_ENDBATTLE_ACK(client.SessionPlayer));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}