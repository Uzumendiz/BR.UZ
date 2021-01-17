using System.Net.Sockets;

namespace PointBlank
{
    public class TcpPacket
    {
        public const int BufferSize = 8976;
        public byte[] Buffer = new byte[BufferSize];
        public Socket WorkSocket;
        public int Length;
        public ushort Opcode;
    }
}