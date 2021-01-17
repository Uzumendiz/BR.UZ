namespace PointBlank.Game
{
    public class VOTEKICK_START_PAK : GamePacketWriter
    {
        private VoteKick vote;
        public VOTEKICK_START_PAK(VoteKick vote)
        {
            this.vote = vote;
        }

        public override void Write()
        {
            WriteH(3399);
            WriteC((byte)vote.creatorIdx);
            WriteC((byte)vote.victimIdx);
            WriteC((byte)vote.motive);
        }
    }
}