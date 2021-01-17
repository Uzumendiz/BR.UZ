namespace PointBlank.Game
{
    public class CLAN_PRIVILEGES_KICK_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(1336);
        }
    }
}