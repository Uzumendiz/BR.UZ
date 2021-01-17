namespace PointBlank.Game
{
    public class LOBBY_QUICKJOIN_ROOM_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3078);
            WriteD(0x80000000);
        }
    }
}