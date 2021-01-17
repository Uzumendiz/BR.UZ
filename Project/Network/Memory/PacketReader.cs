using System;

namespace PointBlank
{
    public class PacketReader
    {
        private byte[] buffer;
        private int offset;
        public PacketReader(byte[] buff)
        {
            buffer = buff;
        }
        public int ReadD()
        {
            int num = BitConverter.ToInt32(buffer, offset);
            offset += 4;
            return num;
        }
        public uint ReadUD()
        {
            uint num = BitConverter.ToUInt32(buffer, offset);
            offset += 4;
            return num;
        }
        public byte ReadC()
        {
            try
            {
                byte num = buffer[offset++];
                return num;
            }
            catch
            {
                return 0;
            }
        }

        public byte[] ReadB(int Length)
        {
            byte[] result = new byte[Length];
            Array.Copy(buffer, offset, result, 0, Length);
            offset += Length;
            return result;
        }

        public short ReadH()
        {
            short num = BitConverter.ToInt16(buffer, offset);
            offset += 2;
            return num;
        }

        public ushort ReadUH()
        {
            ushort num = BitConverter.ToUInt16(buffer, offset);
            offset += 2;
            return num;
        }

        public double ReadF()
        {
            double num = BitConverter.ToDouble(buffer, offset);
            offset += 8;
            return num;
        }
        public float ReadT()
        {
            float num = BitConverter.ToSingle(buffer, offset);
            offset += 4;
            return num;
        }
        public long ReadQ()
        {
            long num = BitConverter.ToInt64(buffer, offset);
            offset += 8;
            return num;
        }
        public ulong ReadQ2()
        {
            ulong num = BitConverter.ToUInt64(buffer, offset);
            offset += 8;
            return num;
        }

        public string ReadS(int Length)
        {
            string str = "";
            try
            {
                str = Settings.EncodingText.GetString(buffer, offset, Length);
                int length = str.IndexOf((char)0);
                if (length != -1)
                {
                    str = str.Substring(0, length);
                }
                offset += Length;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return str;
        }
    }
}