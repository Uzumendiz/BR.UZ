namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_GIVEUPBATTLE_ACK : GamePacketWriter
    {
        private Room room;
        private int oldLeaderSlot;
        public PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(Room room, int oldLeaderSlot)
        {
            this.room = room;
            this.oldLeaderSlot = oldLeaderSlot;
        }

        public override void Write()
        {
            WriteH(3347);
            WriteD(room.leaderSlot);
            for (int i = 0; i < 16; i++)
            {
                if (oldLeaderSlot == i)
                {
                    WriteB(new byte[13]);
                }
                else
                {
                    Account playerSlot = room.GetPlayerBySlot(i);
                    if (playerSlot != null)
                    {
                        WriteIP(playerSlot.ipAddress);
                        WriteH(29890);
                        WriteB(playerSlot.localIP);
                        WriteH(29890);
                        WriteC(0);
                    }
                    else
                    {
                        WriteB(new byte[13]);
                    }
                }
            }
        }
    }
}