namespace PointBlank.Game
{
    public class BOX_MESSAGE_GIFT_RECEIVE_PAK : GamePacketWriter
    {
        private Message gift;
        public BOX_MESSAGE_GIFT_RECEIVE_PAK(Message gift)
        {
            this.gift = gift;
        }

        public override void Write()
        {
            WriteH(553);
            WriteD(gift.objectId); //MsgId?
            WriteD((uint)gift.senderId); //Good
            WriteD(gift.state); //?
            WriteD((uint)gift.expireDate); //Data do término
            WriteC((byte)(gift.senderName.Length + 1));
            WriteS(gift.senderName, gift.senderName.Length + 1);
            WriteC(6);
            WriteS("EVENT", 6); //Mensagem
            /*
             * 29 02 EF 11   ·|×···u····)·ï·
00000090   21 00 A9 30 9A 00 00 00  00 00 BF 6D B2 65 03 47   !·©0·····¿m²e·G
000000A0   4D 00 06 45 56 45 4E 54  00 
             */
        }
    }
}