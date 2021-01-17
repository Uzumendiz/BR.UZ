using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_TEAM_CHATTING_REQ : GamePacketReader
    {
        private ChattingTypeEnum type;
        private string text;
        public override void ReadImplement()
        {
            type = (ChattingTypeEnum)ReadShort();
            text = ReadString(ReadShort());
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                if (p == null || p.match == null || type != ChattingTypeEnum.Match)
                    return;
                Match match = p.match;
                using (CLAN_WAR_TEAM_CHATTING_PAK packet = new CLAN_WAR_TEAM_CHATTING_PAK(p.nickname, text))
                    match.SendPacketToPlayers(packet);
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}