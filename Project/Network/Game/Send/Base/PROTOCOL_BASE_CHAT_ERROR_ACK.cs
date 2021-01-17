namespace PointBlank.Game
{
    public class BASE_CHAT_ERROR_PAK : GamePacketWriter
    {
        private int error, banTime;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error">0=Nenhum bloqueio; 1=?; 2="Primeiro aviso"</param>
        /// <param name="time">Duração do bloqueio</param>
        public BASE_CHAT_ERROR_PAK(int error, int time = 0)
        {
            this.error = error;
            banTime = time;
        }
        public override void Write()
        {
            WriteH(2628);
            WriteC((byte)error); //Result/Type (0 = Não bloqueado | 2 = Primeiro block | 1 = ?)
            if (error > 0)
            {
                WriteD(banTime); //Segundos
            }
            /*
             * 1=STR_MESSAGE_BLOCK_ING
             * 2=STR_MESSAGE_BLOCK_START
             */
        }
    }
}