using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public interface ICommandClass
    {
        IZwaveNode Node { get; }
        CommandClass Class { get; }
        byte Version { get; }
        byte EndPoint { get; }
    }
}
