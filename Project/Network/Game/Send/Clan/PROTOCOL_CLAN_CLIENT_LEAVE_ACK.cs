namespace PointBlank.Game
{
    public class CLAN_CLIENT_LEAVE_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(1444);
        }
    }
}