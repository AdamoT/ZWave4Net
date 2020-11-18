using ZWave.Channel;

namespace ZWave.CommandClasses
{
    internal interface ICommandClassInternal : ICommandClass
    {
        void Initialize(IZwaveNode node, byte endPoint = 0);

        void HandleEvent(Command command);
    }
}
