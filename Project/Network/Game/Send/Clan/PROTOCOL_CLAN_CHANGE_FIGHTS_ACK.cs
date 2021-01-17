namespace PointBlank.Game
{
    public class CLAN_CHANGE_FIGHTS_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(1409);
        }
    }
}