namespace PointBlank.Game
{
    public class VOTEKICK_UPDATE_PAK : GamePacketWriter
    {
        private VoteKick vote;
        public VOTEKICK_UPDATE_PAK(VoteKick vote)
        {
            this.vote = vote;
        }

        public override void Write()
        {
            WriteH(3407);
            WriteC((byte)vote.kikar);
            WriteC((byte)vote.deixar);
        }
    }
}