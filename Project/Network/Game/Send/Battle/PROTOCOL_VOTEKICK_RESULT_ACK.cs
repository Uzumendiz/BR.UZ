namespace PointBlank.Game
{
    public class VOTEKICK_RESULT_PAK : GamePacketWriter
    {
        private VoteKick vote;
        private uint error;
        public VOTEKICK_RESULT_PAK(uint error, VoteKick vote)
        {
            this.error = error;
            this.vote = vote;
        }

        public override void Write()
        {
            WriteH(3403);
            WriteC((byte)vote.victimIdx);
            WriteC((byte)vote.kikar);
            WriteC((byte)vote.deixar);
            WriteD(error);
            //[2147488000] - cancelou a votação
            //[2147488001] - Sem votos aliados
            //[2147488002] - Sem votos adversários
            //[2147488003] - Patente não pode abrir
            //
        }
    }
}