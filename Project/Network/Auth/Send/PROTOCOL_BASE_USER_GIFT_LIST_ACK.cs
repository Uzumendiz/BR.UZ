using System;
using System.Collections.Generic;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_GIFT_LIST_ACK : GamePacketWriter
    {
        private int erro;
        private List<Message> gifts;
        public PROTOCOL_BASE_USER_GIFT_LIST_ACK(int erro, List<Message> gifts)
        {
            this.erro = erro;
            this.gifts = gifts;
        }

        public override void Write()
        {
            WriteH(529);
            WriteD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm"))); //? data
            WriteQ(0); //Nunca é usado.
            WriteD(erro); //Count?/Existe presentes pendentes?|Valor<0  - Cancela o resto dos códigos abaixo
            WriteC(0); //gifts.Count
            WriteC((byte)gifts.Count);
            WriteC(0); //0 = Ativa algo? || O valor deve estar entre 0-99 || Valor acima de 100 = Erro GiftNouteCount Overflow
            foreach (Message gift in gifts)
            {
                WriteD(gift.objectId); //Msg?
                WriteD((uint)gift.senderId); //Good da loja
                WriteD(gift.state); //already get
                WriteD((uint)gift.expireDate); //enddate
                WriteC((byte)(gift.senderName.Length + 1));
                WriteS(gift.senderName, gift.senderName.Length + 1);
                WriteC(1);
                WriteS("", 1);
            }
        }
    }
}