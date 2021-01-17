using System;

namespace PointBlank
{
    public class BattlePacketReader
    {
        private readonly byte[] buffer;
        private int offset;
        public BattlePacketReader(byte[] buffer)
        {
            this.buffer = buffer;
        }

        protected internal void Advance(int bytes)
        {
            offset += bytes;
            if (offset > buffer.Length)
            {
                Logger.Error($" [BattlePacketReader] Offset ultrapassou o valor do buffer. ({offset}/{buffer.Length})");
            }
        }
        protected internal int ReadInt()
        {
            int num = BitConverter.ToInt32(buffer, offset);
            offset += 4;
            return num;
        }
        protected internal uint ReadUint()
        {
            uint num = BitConverter.ToUInt32(buffer, offset);
            offset += 4;
            return num;
        }
        protected internal Half3 ReadUshortVector()
        {
            return new Half3(ReadUshort(), ReadUshort(), ReadUshort());
        }
        protected internal Half3 ReadFloatVector()
        {
            return new Half3(ReadFloat(), ReadFloat(), ReadFloat());
        }
        protected internal byte ReadByteOutException(out bool exception)
        {
            try
            {
                exception = false;
                return buffer[offset++];
            }
            catch
            {
                exception = true;
                return 0;
            }
        }
        protected internal byte ReadByte()
        {
            return buffer[offset++];
        }
        protected internal byte[] ReadB(int Length)
        {
            byte[] result = new byte[Length];
            Array.Copy(buffer, offset, result, 0, Length);
            offset += Length;
            return result;
        }
        protected internal long ReadLong()
        {
            long num = BitConverter.ToInt64(buffer, offset);
            offset += 8;
            return num;
        }
        protected internal ulong ReadUlong()
        {
            ulong num = BitConverter.ToUInt64(buffer, offset);
            offset += 8;
            return num;
        }
        protected internal short ReadShort()
        {
            short num = BitConverter.ToInt16(buffer, offset);
            offset += 2;
            return num;
        }
        protected internal ushort ReadUshort()
        {
            ushort num = BitConverter.ToUInt16(buffer, offset);
            offset += 2;
            return num;
        }
        protected internal float ReadFloat()
        {
            float num = BitConverter.ToSingle(buffer, offset);
            offset += 4;
            return num;
        }
    }
}