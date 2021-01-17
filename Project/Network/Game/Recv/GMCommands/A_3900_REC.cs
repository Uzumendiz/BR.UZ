using System;

namespace PointBlank.Game
{
    public class PROTOCOL_A_3900_REQ : GamePacketReader
    {
        private int Slot;
        private string Reason;
        public override void ReadImplement()
        {
            Slot = ReadByte();
            Reason = ReadString(ReadByte());
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
                //Ativa quando usa "/BLOCK (SLOT) (REASON)"
                Logger.Warning("[3900] Slot: " + Slot + "; Reason: " + Reason);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}