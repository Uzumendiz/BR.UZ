namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_REQUEST_INFO_ACK : GamePacketWriter
    {
        private string text;
        private uint error;
        private Account playerInfo;
        public PROTOCOL_CLAN_REQUEST_INFO_ACK(Account playerInfo, string text, uint error)
        {
            this.playerInfo = playerInfo;
            this.text = text;
            this.error = error;
        }

        public PROTOCOL_CLAN_REQUEST_INFO_ACK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(1325);
            WriteD(error);
            if (error == 0)
            {
                WriteQ(playerInfo.playerId);
                WriteS(playerInfo.nickname, 33);
                WriteC(playerInfo.rankId);
                WriteD(playerInfo.statistics.kills);
                WriteD(playerInfo.statistics.deaths);
                WriteD(playerInfo.statistics.fights);
                WriteD(playerInfo.statistics.fightsWin);
                WriteD(playerInfo.statistics.fightsLost);
                WriteS(text, text.Length + 1);
            }
        }
    }
}