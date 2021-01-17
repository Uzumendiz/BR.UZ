using System;

namespace PointBlank.Game
{
    public class PROTOCOL_A_3094_REQ : GamePacketReader
    {
        private int sessionId;
        private string name;
        public override void ReadImplement()
        {                
            //Ativa quando usa "/EXIT (APELIDO)"
            sessionId = ReadInt();
            name = ReadString(ReadByte());
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                Channel ch = p != null ? p.GetChannel() : null;
                if (ch == null || p.room != null || sessionId == int.MaxValue)
                {
                    return;
                }
                PlayerSession pS = ch.GetPlayer(sessionId);
                if (pS == null)
                {
                    return;
                }
                Account pC = AccountManager.GetAccount(pS.playerId, true);
                if (pC == null)
                {
                    return;
                }
                Logger.Warning("[3094] SessionId: " + sessionId + "; Name: " + name);
                //_client.SendPacket(new A_3094_PAK());
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}