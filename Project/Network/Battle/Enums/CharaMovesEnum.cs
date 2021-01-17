﻿using System;

namespace PointBlank
{
    [Flags]
    public enum CharaMovesEnum
    {
        STOPPED = 0,
        LEFT = 1,
        BACK = 2,
        RIGHT = 4,
        FRONT = 8,
        HELI_IN_MOVE = 16,
        HELI_UNKNOWN = 32,
        HELI_LEAVE = 64,
        HELI_STOPPED = 128
    }
}