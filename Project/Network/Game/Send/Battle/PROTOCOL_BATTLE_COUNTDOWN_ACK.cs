namespace PointBlank.Game
{
    public class BATTLE_COUNTDOWN_PAK : GamePacketWriter
    {
        private CountDownEnum timer;
        public BATTLE_COUNTDOWN_PAK(CountDownEnum timer)
        {
            this.timer = timer;
        }
        public override void Write()
        {
            WriteH(3340);
            WriteC((byte)timer);
        }
    }
}