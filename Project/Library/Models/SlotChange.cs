namespace PointBlank
{
    public class SlotChange
    {
        public Slot oldSlot;
        public Slot newSlot;
        public SlotChange(Slot oldSlot, Slot newSlot)
        {
            this.oldSlot = oldSlot;
            this.newSlot = newSlot;
        }
    }
}