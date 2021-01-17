using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PointBlank
{
    public class BattlePacketWriter : IDisposable
    {
        public MemoryStream memory = new MemoryStream();
        private bool disposed = false;
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        protected internal void WriteB(byte[] value)
        {
            memory.Write(value, 0, value.Length);
        }
        protected internal void WriteD(int value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteD(uint value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteH(short val)
        {
            WriteB(BitConverter.GetBytes(val));
        }
        protected internal void WriteH(ushort val)
        {
            WriteB(BitConverter.GetBytes(val));
        }
        protected internal void WriteC(bool value) //True = 1; False = 0.
        {
            memory.WriteByte(Convert.ToByte(value));
        }
        protected internal void WriteC(byte value)
        {
            memory.WriteByte(value);
        }
        protected internal void WriteT(float value)
        {
            WriteB(BitConverter.GetBytes(value));
        }

        protected internal void WriteHVector(Half3 half)
        {
            WriteH(half.X.RawValue);
            WriteH(half.Y.RawValue);
            WriteH(half.Z.RawValue);
        }
        protected internal void WriteTVector(Half3 half)
        {
            WriteT(half.X);
            WriteT(half.Y);
            WriteT(half.Z);
        }

        /// <summary>
        /// Volta uma determinada quantia de bytes do MemoryStream.
        /// </summary>
        /// <param name="value"></param>
        protected internal void GoBack(int value) => memory.Position -= value;
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
            memory.Dispose();
            if (disposing)
            {
                handle.Dispose();
                handle = null;
            }
            disposed = true;
        }
    }
}