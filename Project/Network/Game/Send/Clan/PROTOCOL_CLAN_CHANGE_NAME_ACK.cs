namespace PointBlank.Game
{
    public class CLAN_CHANGE_NAME_PAK : GamePacketWriter
    {
        private string _name;
        public CLAN_CHANGE_NAME_PAK(string name)
        {
            _name = name;
        }

        public override void Write()
        {
            WriteH(1368);
            WriteS(_name, 17);
        }
    }
}