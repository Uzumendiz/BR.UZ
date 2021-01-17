using System;

namespace PointBlank.Game
{
    public class BASE_2626_PAK : GamePacketWriter
    {
        private Account player;
        public BASE_2626_PAK(Account player)
        {
            this.player = player;
        }

        public override void Write()
        {
            WriteH(2626);
            WriteB(BitConverter.GetBytes(player.playerId), 0, 4);
            WriteQ(player.titles.Flags);
            WriteC(player.titles.Equiped1);
            WriteC(player.titles.Equiped2);
            WriteC(player.titles.Equiped3);
            WriteD(player.titles.Slots);
        }
    }
}