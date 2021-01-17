namespace PointBlank
{
    public class ActionModel
    {
        public ushort Slot;
        public ushort LengthData;
        public byte[] Data;
        public EventsEnum Flags;
        public byte Type; //P2PSubHeadEnum
    }
}