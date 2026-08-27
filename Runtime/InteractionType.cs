using System;

namespace YusufShabanov.InteractionSystem
{
    [Flags]
    public enum InteractionType
    {
        None = 0,
        Everything = ~0,
        Press = 1 << 0,
        Hold = 1 << 1,
        Tap = 1 << 2,
        MultiTap = 1 << 3,
        SlowTap = 1 << 4
    }
}
