using System.Collections.Generic;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_CLAN_MEMBERS_ACK : GamePacketWriter
    {
        private List<Account> players;
        public PROTOCOL_BASE_USER_CLAN_MEMBERS_ACK(List<Account> players)
        {
            this.players = players;
        }

        public override void Write()
        {
            WriteH(1349);
            WriteC((byte)players.Count);
            for (int i = 0; i < players.Count; i++)
            {
                Account member = players[i];
                WriteC((byte)(member.nickname.Length + 1));
                WriteS(member.nickname, member.nickname.Length + 1);
                WriteQ(member.playerId);
                WriteQ(Utilities.GetClanStatus(member.status, member.isOnline));
                WriteC(member.rankId);
            }
        }
    }
}