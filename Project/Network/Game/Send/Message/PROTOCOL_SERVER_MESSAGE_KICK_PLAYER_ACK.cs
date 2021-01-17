namespace PointBlank.Game
{
    public class SERVER_MESSAGE_KICK_PLAYER_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2051);
        }
    }
}