using System.Net.Sockets;

namespace PointBlank
{
    public class StateObject
    {
        public const int BufferSize = 8096;
        public byte[] Buffer = new byte[BufferSize];
        public Socket WorkSocket;
    }
}