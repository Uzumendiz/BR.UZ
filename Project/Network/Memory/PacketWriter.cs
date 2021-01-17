using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace PointBlank
{
    public class PacketWriter : IDisposable
    {
        public MemoryStream memorystream = new MemoryStream();
        private bool disposed = false;
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            memorystream.Dispose();
            if (disposing)
            {
                handle.Dispose();
                handle = null;
            }
            disposed = true;
        }
        public PacketWriter()
        {
        }
        public PacketWriter(long length)
        {
            memorystream.SetLength(length);
        }
        public void WriteIP(string address)
        {
            WriteB(IPAddress.Parse(address).GetAddressBytes());
        }
        public void WriteB(byte[] value)
        {
            memorystream.Write(value, 0, value.Length);
        }
        public void WriteB(byte[] value, int offset, int length)
        {
            memorystream.Write(value, offset, length);
        }
        public void WriteD(int value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        public void WriteD(uint value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        public void WriteH(short value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        public void WriteH(ushort value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        public void WriteH(int offset, ushort value)
        {
            memorystream.Position = offset;
            WriteB(BitConverter.GetBytes(value));
        }
        public void WriteC(byte value)
        {
            memorystream.WriteByte(value);
        }
        public void WriteC(int offset, byte value)
        {
            memorystream.Position = offset;
            memorystream.WriteByte(value);
        }
        public void WriteC(bool value)
        {
            memorystream.WriteByte(Convert.ToByte(value));
        }
        public void WriteF(double value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        public void WriteQ(long value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        public void WriteQ(ulong value)
        {
            WriteB(BitConverter.GetBytes(value));
        }

        public void WriteS(string name, int count)
        {
            if (name == null)
            {
                return;
            }
            WriteB(Settings.EncodingText.GetBytes(name));
            WriteB(new byte[count - name.Length]);
        }
    }
}