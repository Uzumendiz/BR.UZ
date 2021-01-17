namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_TIMERSYNC_ACK : GamePacketWriter
    {
        private Room room;
        public PROTOCOL_BATTLE_TIMERSYNC_ACK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3371);
            WriteC(room.rounds);
            WriteD(room.GetInBattleTimeLeft());
            WriteH(room.GetSlotsFlag(true, false));
        }
    }
}