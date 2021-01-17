namespace PointBlank
{
    public abstract class Client
    {
        public abstract string GetIPAddress();
        public abstract void SendCompletePacket(byte[] data);
        public abstract void SendPacket(GamePacketWriter ACK);
        public abstract void Close(int time = 0, bool destroyOrKickByGm = false);
    }

    public abstract class ClientApi
    {
        public abstract string GetIPAddress();
        public abstract void SendCompletePacket(byte[] data);
        public abstract void SendPacket(ApiPacketWriter ACK);
        public abstract void Close(int time = 0);
    }
}