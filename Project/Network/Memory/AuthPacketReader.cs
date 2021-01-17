using System;

namespace PointBlank
{
    public abstract class AuthPacketReader
    {
        public AuthClient client;
        public byte[] buffer;
        private int offset = 4;
        public abstract void ReadImplement();
        public abstract void RunImplement();
        public void Log(Exception ex)
        {
            Logger.Error(" [AuthPacketReader] Exception: " + ex);
            if (client != null)
            {
                client.Close();
            }
        }
        public void PacketLog(Exception ex)
        {
            Logger.Exception(ex);
            if (client != null)
            {
                client.Close(1000);
            }
        }
        protected internal int ReadInt()
        {
            try
            {
                int num = BitConverter.ToInt32(buffer, offset);
                offset += 4;
                return num;
            }
            catch (Exception ex)
            {
                Log(ex);
                return 0;
            }
        }

        protected internal int ReadIntOutException(out bool exception)
        {
            try
            {
                int num = BitConverter.ToInt32(buffer, offset);
                offset += 4;
                exception = false;
                return num;
            }
            catch
            {
                exception = true;
                return -1;
            }
        }

        protected internal uint ReadUint()
        {
            try
            {
                uint num = BitConverter.ToUInt32(buffer, offset);
                offset += 4;
                return num;
            }
            catch (Exception ex)
            {
                Log(ex);
                return 0;
            }
        }

        protected internal byte ReadByte()
        {
            try
            {
                byte num = buffer[offset++];
                return num;
            }
            catch (Exception ex)
            {
                Log(ex);
                return 0;
            }
        }
        protected internal byte ReadByteJoinRoom()
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

        protected internal byte[] ReadB(int Length)
        {
            try
            {
                byte[] result = new byte[Length];
                Array.Copy(buffer, offset, result, 0, Length);
                offset += Length;
                return result;
            }
            catch (Exception ex)
            {
                Log(ex);
                return new byte[0];
            }
        }

        protected internal short ReadShort()
        {
            try
            {
                short num = BitConverter.ToInt16(buffer, offset);
                offset += 2;
                return num;
            }
            catch (Exception ex)
            {
                Log(ex);
                return 0;
            }
        }

        protected internal ushort ReadUshort()
        {
            try
            {
                ushort num = BitConverter.ToUInt16(buffer, offset);
                offset += 2;
                return num;
            }
            catch (Exception ex)
            {
                Log(ex);
                return 0;
            }
        }

        protected internal double ReadDouble()
        {
            try
            {
                double num = BitConverter.ToDouble(buffer, offset);
                offset += 8;
                return num;
            }
            catch (Exception ex)
            {
                Log(ex);
                return 0;
            }
        }

        protected internal float ReadFloat()
        {
            try
            {
                float num = BitConverter.ToSingle(buffer, offset);
                offset += 4;
                return num;
            }
            catch (Exception ex)
            {
                Log(ex);
                return 0;
            }
        }

        protected internal long ReadLong()
        {
            try
            {
                long num = BitConverter.ToInt64(buffer, offset);
                offset += 8;
                return num;
            }
            catch (Exception ex)
            {
                Log(ex);
                return 0;
            }
        }

        protected internal ulong ReadUlong()
        {
            try
            {
                ulong num = BitConverter.ToUInt64(buffer, offset);
                offset += 8;
                return num;
            }
            catch (Exception ex)
            {
                Log(ex);
                return 0;
            }
        }

        protected internal string ReadString(int Length)
        {
            string str = "";
            try
            {
                str = Settings.EncodingText.GetString(buffer, offset, Length);
                int length = str.IndexOf(char.MinValue);
                if (length != -1)
                {
                    str = str.Substring(0, length);
                }
                offset += Length;
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            return str;
        }
    }
}