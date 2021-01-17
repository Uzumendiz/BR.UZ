using System;

namespace PointBlank
{
    [Flags]
    public enum DeadEnum
    {
        isAlive = 1,
        isDead = 2,
        useChat = 4
    }
}