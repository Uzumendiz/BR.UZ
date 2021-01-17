using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_GET_LOBBY_USER_LIST_ACK : GamePacketWriter
    {
        private List<Account> players;
        private List<int> playersIdxs;
        public PROTOCOL_ROOM_GET_LOBBY_USER_LIST_ACK(Channel ch)
        {
            players = ch.GetWaitPlayers();
            playersIdxs = GetRandomIndexes(players.Count, players.Count >= 8 ? 8 : players.Count);
        }
        private List<int> GetRandomIndexes(int total, int count)
        {
            if (total == 0 || count == 0)
            {
                return new List<int>();
            }
            Random random = new Random();
            List<int> numeros = new List<int>();
            for (int i = 0; i < total; i++)
            {
                numeros.Add(i);
            }
            for (int i = 0; i < numeros.Count; i++)
            {
                int randomized = random.Next(numeros.Count);
                int temp = numeros[i];
                numeros[i] = numeros[randomized];
                numeros[randomized] = temp;
            }
            return numeros.GetRange(0, count);
        }
        public override void Write()
        {
            WriteH(3855);
            WriteD(playersIdxs.Count);
            for (int i = 0; i < playersIdxs.Count; i++)
            {
                Account player = players[playersIdxs[i]];
                byte nickLength = (byte)(player.nickname.Length + 1);
                WriteD(player.GetSessionId());
                WriteD(player.GetRank());
                WriteC(nickLength);
                WriteS(player.nickname, nickLength);
            }
            players = null;
            playersIdxs = null;
        }
    }
}