namespace PointBlank
{
    public class SlotMatch
    {
        public SlotMatchStateEnum state;
        public long playerId;
        public long id;
        public SlotMatch(int slot)
        {
            id = slot;
        }
    }
}