using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public interface ICommandClass_OLD
    {
        IZwaveNode Node { get; }
        CommandClass Class { get; }
    }
}
