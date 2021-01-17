namespace PointBlank.Game
{
    public class BASE_CHANNEL_LIST_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2572);
            WriteD(10); //channelsCount
            WriteD(Settings.MaxPlayersChannel);
            for (int i = 0; i < 10; i++)
            {
                WriteD(ServersManager.channels[i].players.Count);
            }
        }
    }
}