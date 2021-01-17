namespace PointBlank.Game
{
    public class EVENT_VISIT_CONFIRM_PAK : GamePacketWriter
    {
        private EventVisitModel eventVisit;
        private PlayerEvent playerEvent;
        private uint error;
        public EVENT_VISIT_CONFIRM_PAK(EventErrorEnum error, EventVisitModel eventVisit, PlayerEvent playerEvent)
        {
            this.error = (uint)error;
            this.eventVisit = eventVisit;
            this.playerEvent = playerEvent;
        }

        public override void Write()
        {
            WriteH(2662);
            WriteD(error);
            if (error == 0x80001504)
            {
                WriteD(eventVisit.id);
                WriteC(playerEvent.LastVisitSequence1);
                WriteC(playerEvent.LastVisitSequence2);
                WriteH(0xFFFF);
                WriteD(eventVisit.startDate);
            }
        }
    }
}