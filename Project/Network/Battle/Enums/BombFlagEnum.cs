using System;

namespace PointBlank
{
    [Flags]
    public enum BombFlagEnum
    {
        Start = 1,
        Stop = 2,
        Defuse = 4,
        Unk1 = 8
    }
}