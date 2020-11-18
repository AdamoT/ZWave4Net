using ZWave.Channel;

namespace ZWave
{
    internal interface IZwaveNodeInternal : IZwaveNode
    {
        void HandleEvent(Command command);
        void HandleUpdate();
    }
}
