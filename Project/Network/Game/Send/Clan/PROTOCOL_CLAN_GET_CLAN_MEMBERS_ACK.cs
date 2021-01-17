using System.Collections.Generic;

namespace PointBlank.Game
{
    public class CLAN_GET_CLAN_MEMBERS_PAK : GamePacketWriter
    {
        private List<Account> _players;
        public CLAN_GET_CLAN_MEMBERS_PAK(List<Account> players)
        {
            _players = players;
        }
        public override void Write()
        {
            WriteH(1349);
            WriteC((byte)_players.Count);
            for (int i = 0; i < _players.Count; i++)
            {
                Account member = _players[i];
                WriteC((byte)(member.nickname.Length + 1));
                WriteS(member.nickname, member.nickname.Length + 1);
                WriteQ(member.playerId);
                WriteQ(Utilities.GetClanStatus(member.status, member.isOnline));
                WriteC(member.rankId);
            }
        }
    }
}