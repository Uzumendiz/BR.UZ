using System.Runtime.InteropServices;

namespace PointBlank
{
    public class HalfUtils
    {
        private readonly static uint[] HalfToFloatMantissaTable = new uint[2048];
        private readonly static uint[] HalfToFloatExponentTable = new uint[64];
        private readonly static uint[] HalfToFloatOffsetTable = new uint[64];
        private readonly static ushort[] FloatToHalfBaseTable = new ushort[512];
        private readonly static byte[] FloatToHalfShiftTable = new byte[512];
        public static void Load()
        {
            HalfToFloatMantissaTable[0] = 0;
            for (int i = 1; i < 1024; i++)
            {
                uint num = (uint)(i << 13);
                uint num1 = 0;
                while ((num & 8388608) == 0)
                {
                    num1 -= 8388608;
                    num <<= 1;
                }
                num &= 8388609;
                num1 += 947912704;
                HalfToFloatMantissaTable[i] = num | num1;
            }
            for (int i = 1024; i < 2048; i++)
            {
                HalfToFloatMantissaTable[i] = (uint)(939524096 + (i - 1024 << 13));
            }
            HalfToFloatExponentTable[0] = 0;
            for (int i = 1; i < 63; i++)
            {
                if (i >= 31)
                {
                    HalfToFloatExponentTable[i] = (uint)(-2147483648 + (i - 32 << 23));
                }
                else
                {
                    HalfToFloatExponentTable[i] = (uint)(i << 23);
                }
            }
            HalfToFloatExponentTable[31] = 1199570944;
            HalfToFloatExponentTable[32] = 2147483648;
            HalfToFloatExponentTable[63] = 947912704;
            HalfToFloatOffsetTable[0] = 0;
            for (int i = 1; i < 64; i++)
            {
                HalfToFloatOffsetTable[i] = 1024;
            }
            HalfToFloatOffsetTable[32] = 0;
            for (int i = 0; i < 256; i++)
            {
                int num2 = i - 127;
                if (num2 < -24)
                {
                    FloatToHalfBaseTable[i | 0] = 0;
                    FloatToHalfBaseTable[i | 256] = 32768;
                    FloatToHalfShiftTable[i | 0] = 24;
                    FloatToHalfShiftTable[i | 256] = 24;
                }
                else if (num2 < -14)
                {
                    FloatToHalfBaseTable[i | 0] = (ushort)(1024 >> (-num2 - 14 & 31));
                    FloatToHalfBaseTable[i | 256] = (ushort)(1024 >> (-num2 - 14 & 31) | 32768);
                    FloatToHalfShiftTable[i | 0] = (byte)(-num2 - 1);
                    FloatToHalfShiftTable[i | 256] = (byte)(-num2 - 1);
                }
                else if (num2 <= 15)
                {
                    FloatToHalfBaseTable[i | 0] = (ushort)(num2 + 15 << 10);
                    FloatToHalfBaseTable[i | 256] = (ushort)(num2 + 15 << 10 | 32768);
                    FloatToHalfShiftTable[i | 0] = 13;
                    FloatToHalfShiftTable[i | 256] = 13;
                }
                else if (num2 >= 128)
                {
                    FloatToHalfBaseTable[i | 0] = 31744;
                    FloatToHalfBaseTable[i | 256] = 64512;
                    FloatToHalfShiftTable[i | 0] = 13;
                    FloatToHalfShiftTable[i | 256] = 13;
                }
                else
                {
                    FloatToHalfBaseTable[i | 0] = 31744;
                    FloatToHalfBaseTable[i | 256] = 64512;
                    FloatToHalfShiftTable[i | 0] = 24;
                    FloatToHalfShiftTable[i | 256] = 24;
                }
            }
        }

        public static ushort Pack(float f)
        {
            FloatToUint floatToUint = new FloatToUint()
            {
                floatValue = f
            };
            return (ushort)(FloatToHalfBaseTable[floatToUint.uintValue >> 23 & 511] + ((floatToUint.uintValue & 8388607) >> (FloatToHalfShiftTable[floatToUint.uintValue >> 23 & 511] & 31)));
        }

        public static float Unpack(ushort h)
        {
            FloatToUint floatToUint = new FloatToUint()
            {
                uintValue = HalfToFloatMantissaTable[HalfToFloatOffsetTable[h >> 10] + (h & 1023)] + HalfToFloatExponentTable[h >> 10]
            };
            return floatToUint.floatValue;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatToUint
        {
            [FieldOffset(0)]
            public uint uintValue;

            [FieldOffset(0)]
            public float floatValue;
        }
    }
}