using System.Net;
using System.Net.Sockets;

namespace PointBlank
{
    public class UdpPacket
    {
        public const int BufferSize = 8096;
        public byte[] Buffer = new byte[BufferSize];
        public UdpClient WorkUdp;
        public int Length;
        public ushort Opcode;
        public IPEndPoint RemoteEndPoint;
    }
}