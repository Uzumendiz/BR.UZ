using System.Collections.Generic;

namespace PointBlank
{
    public class VoteKick
    {
        public int creatorIdx;
        public int victimIdx;
        public int motive;
        public int kikar = 1;
        public int deixar = 1;
        public int allies;
        public int enemys;
        public List<int> votes = new List<int>();
        public bool[] TotalArray = new bool[16];

        /// <summary>
        /// Define o 'creatorIdx' e 'victimIdx'.
        /// <para>Adiciona os 2 jogadores na lista de votos.</para>
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="victim"></param>
        public VoteKick(int creator, int victim)
        {
            creatorIdx = creator;
            victimIdx = victim;
            votes.Add(creator);
            votes.Add(victim);
        }

        /// <summary>
        /// Retorna a quantidade de jogadores que votaram e ainda estão na partida.
        /// </summary>
        /// <returns></returns>
        public int GetInGamePlayers()
        {
            int count = 0;
            for (int i = 0; i < 16; i++)
            {
                if (TotalArray[i])
                {
                    count++;
                }
            }
            return count;
        }
    }
}