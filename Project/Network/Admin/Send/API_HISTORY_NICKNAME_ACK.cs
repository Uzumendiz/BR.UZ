using System.Collections.Generic;

namespace PointBlank.Api
{
    public class API_HISTORY_NICKNAME_ACK : ApiPacketWriter
    {
        private List<NHistoryModel> historys;
        public API_HISTORY_NICKNAME_ACK(List<NHistoryModel> historys)
        {
            this.historys = historys;
        }

        public override void Write()
        {
            WriteH(16);
            WriteC((byte)historys.Count);
            foreach (NHistoryModel history in historys)
            {
                WriteQ(history.player_id);
                WriteC((byte)history.from_nick.Length);
                WriteS(history.from_nick, history.from_nick.Length);
                WriteC((byte)history.to_nick.Length);
                WriteS(history.to_nick, history.to_nick.Length);
                WriteQ(history.date);
                WriteC((byte)history.motive.Length);
                WriteS(history.motive, history.motive.Length);
            }
        }
    }
}