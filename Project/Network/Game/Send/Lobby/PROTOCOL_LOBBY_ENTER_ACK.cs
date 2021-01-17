namespace PointBlank.Game
{
    public class LOBBY_ENTER_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3080);
            WriteD(0);
        }
    }
}