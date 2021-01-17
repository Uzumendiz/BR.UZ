using System;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_ENTER_REQ : AuthPacketReader
    {
        private string login;
        private byte[] localIP;
        private long playerId;
        private int rede;
        public override void ReadImplement()
        {
            login = ReadString(ReadByte());
            playerId = ReadLong();
            rede = ReadByte();
            localIP = ReadB(4);
        }

        public override void RunImplement()
        {
            try
            {
                //Este pacote é chamado caso o cliente tente se conectar a porta do game e não tenha sucesso por algum bloqueio de firewall etc.. ele chama este pacote na mesma conexão do auth.
                Logger.Warning($" [Auth] [BASE_USER_ENTER_REQ] Login: {login} PlayerId: {playerId} Login: {login}");
                client.SendCompletePacket(PackageDataManager.BASE_USER_ENTER_ERROR_PAK);
                client.Close();
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}