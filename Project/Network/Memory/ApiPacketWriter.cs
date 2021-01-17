using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace PointBlank
{
    public abstract class ApiPacketWriter : IDisposable
    {
        public MemoryStream memorystream = new MemoryStream();
        private bool disposed = false;
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public byte[] GetCompleteBytes(string name)
        {
            try
            {
                Write();
                return memorystream.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error(" [ApiPacketWriter] [GetCompleteBytes] Method: " + name + "\r\n" + ex);
            }
            return new byte[0];
        }

        public abstract void Write();
        protected internal void WriteIP(string address)
        {
            WriteB(IPAddress.Parse(address).GetAddressBytes());
        }
        protected internal void WriteIP(IPAddress address)
        {
            WriteB(address.GetAddressBytes());
        }
        protected internal void WriteB(byte[] value)
        {
            memorystream.Write(value, 0, value.Length);
        }
        protected internal void WriteB(byte[] value, int offset, int length)
        {
            memorystream.Write(value, offset, length);
        }
        protected internal void WriteD(bool value)
        {
            WriteB(new byte[] { Convert.ToByte(value), 0, 0, 0 });
        }
        protected internal void WriteD(uint valor)
        {
            WriteB(BitConverter.GetBytes(valor));
        }
        protected internal void WriteD(int value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteH(ushort valor)
        {
            WriteB(BitConverter.GetBytes(valor));
        }
        protected internal void WriteH(short val)
        {
            WriteB(BitConverter.GetBytes(val));
        }
        protected internal void WriteC(byte value)
        {
            memorystream.WriteByte(value);
        }
        protected internal void WriteC(bool value)
        {
            memorystream.WriteByte(Convert.ToByte(value));
        }
        protected internal void WriteT(float value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteF(double value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteQ(ulong valor)
        {
            WriteB(BitConverter.GetBytes(valor));
        }
        protected internal void WriteQ(long valor)
        {
            WriteB(BitConverter.GetBytes(valor));
        }
        protected internal void WriteS(string value)
        {
            if (value != null)
            {
                WriteB(Encoding.Unicode.GetBytes(value));
            }
        }
        protected internal void WriteS(string name, int count)
        {
            if (name == null)
            {
                return;
            }
            WriteB(Settings.EncodingText.GetBytes(name));
            WriteB(new byte[count - name.Length]);
        }
        protected internal void WriteS(string name, int count, int CodePage)
        {
            if (name == null)
            {
                return;
            }
            WriteB(Encoding.GetEncoding(CodePage).GetBytes(name));
            WriteB(new byte[count - name.Length]);
        }

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
            memorystream.Close();
            memorystream.Dispose();
            memorystream = null;
            if (disposing)
            {
                handle.Dispose();
                handle = null;
            }
            disposed = true;
        }
    }
}