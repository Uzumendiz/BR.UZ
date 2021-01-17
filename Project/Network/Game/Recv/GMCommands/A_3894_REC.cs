using System;

namespace PointBlank.Game
{
    public class PROTOCOL_A_3894_REQ : GamePacketReader
    {
        private int Slot;
        public override void ReadImplement()
        {
            Slot = ReadByte();
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
                //Ativa quando usa "/EXIT (SLOT)"
                Logger.Warning(" [A_3894_REQ] /EXIT Slot: " + Slot);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}