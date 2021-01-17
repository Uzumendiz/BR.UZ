using System;

namespace PointBlank.Game
{
    public class PROTOCOL_GM_LOG_LOBBY_REQ : GamePacketReader
    {
        private int sessionId;
        public override void ReadImplement()
        {
            sessionId = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || !player.IsGM())
                {
                    return;
                }
                Account p = null;
                try
                {
                    p = AccountManager.GetAccount(player.GetChannel().GetPlayer(sessionId).playerId, true);
                }
                catch
                {
                }
                if (p != null)
                {
                    client.SendPacket(new GM_LOG_LOBBY_PAK(p));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}