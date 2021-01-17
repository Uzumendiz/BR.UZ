﻿namespace PointBlank
{
    public struct Half
    {
        public ushort RawValue;
        public Half(float value)
        {
            RawValue = HalfUtils.Pack(value);
        }

        public Half(ushort rawvalue)
        {
            RawValue = rawvalue;
        }

        public static implicit operator float(Half value)
        {
            return HalfUtils.Unpack(value.RawValue);
        }
    }
}
