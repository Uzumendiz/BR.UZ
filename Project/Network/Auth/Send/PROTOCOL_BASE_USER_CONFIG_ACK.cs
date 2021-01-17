namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_CONFIG_ACK : GamePacketWriter
    {
        private int error;
        private PlayerConfig config;
        private bool isValid;
        public PROTOCOL_BASE_USER_CONFIG_ACK(int error, PlayerConfig config)
        {
            this.error = error;
            this.config = config;
            isValid = config != null;
        }

        public override void Write()
        {
            WriteH(2568);
            WriteD(error);
            if (error < 0)
            {
                return;
            }
            WriteC(!isValid); //1= Default | 0 = Customizable
            if (isValid)
            {
                WriteH(config.blood);
                WriteC(config.sight);
                WriteC(config.hand);
                WriteD(config.config);
                WriteD(config.audioEnable);
                WriteH(0);
                WriteC(config.audio);
                WriteC(config.music);
                WriteC((byte)config.fov);
                WriteC(0);
                WriteC(config.sensibilidade);
                WriteC(config.invertedMouse);
                WriteH(0);
                WriteC(config.messageInvitation);
                WriteC(config.chatPrivate);
                WriteD(config.macros);
                WriteB(new byte[] { 0, 57, 248, 16, 0 });
                WriteB(config.keys);
            
                byte macro1 = (byte)(config.macro_1.Length + 1);
                byte macro2 = (byte)(config.macro_2.Length + 1);
                byte macro3 = (byte)(config.macro_3.Length + 1);
                byte macro4 = (byte)(config.macro_4.Length + 1);
                byte macro5 = (byte)(config.macro_5.Length + 1);
                WriteC(macro1);
                WriteS(config.macro_1, macro1);
                WriteC(macro2);
                WriteS(config.macro_2, macro2);
                WriteC(macro3);
                WriteS(config.macro_3, macro3);
                WriteC(macro4);
                WriteS(config.macro_4, macro4);
                WriteC(macro5);
                WriteS(config.macro_5, macro5);
            }
        }
    }
}