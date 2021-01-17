using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHANGE_SLOTS_ACK : GamePacketWriter
    {
        private List<SlotChange> slots;
        private int type, leaderSlot;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slots"></param>
        /// <param name="leaderSlot"></param>
        /// <param name="type">0=Normal; 1=Slots ajustados por balanceamento; 2=Dono da sala trocou os times</param>
        public PROTOCOL_ROOM_CHANGE_SLOTS_ACK(List<SlotChange> slots, int leaderSlot, int type)
        {
            this.slots = slots;
            this.leaderSlot = leaderSlot;
            this.type = type;
        }

        public override void Write()
        {
            WriteH(3877);
            WriteC((byte)type);
            WriteC((byte)leaderSlot);
            WriteC((byte)slots.Count);
            for (int i = 0; i < slots.Count; i++)
            {
                SlotChange slot = slots[i];
                WriteC((byte)slot.oldSlot.Id);
                WriteC((byte)slot.newSlot.Id);
                WriteC((byte)slot.oldSlot.state);
                WriteC((byte)slot.newSlot.state);
            }
        }
    }
}