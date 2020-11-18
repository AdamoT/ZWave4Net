using System;

namespace ZWave
{
    [Flags]
    public enum NodeCapabilities
    {
        Routing = 1 << 6,
        Listening = 1 << 7
    }
}
