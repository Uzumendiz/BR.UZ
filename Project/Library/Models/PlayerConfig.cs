namespace PointBlank
{
    public class PlayerConfig
    {
        public byte sight;
        public byte audio = 100;
        public byte music = 60;
        public byte sensibilidade = 50;
        public int fov = 70;
        public short blood = 1;
        public byte hand;
        public int audioEnable = 7;
        public int config = 55;
        public byte invertedMouse;
        public byte messageInvitation;
        public byte chatPrivate;
        public int macros = 31;

        public string macro_1 = "";
        public string macro_2 = "";
        public string macro_3 = "";
        public string macro_4 = "";
        public string macro_5 = "";

        public byte[] keys = new byte[215];
    }
}