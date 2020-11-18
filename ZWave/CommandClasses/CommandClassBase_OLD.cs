using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class CommandClassBase_OLD : ICommandClass_OLD
    {
        public CommandClassBase_OLD(IZwaveNode node, CommandClass @class)
        {
            Node = node;
            Class = @class;
        }

        protected ZWaveChannel Channel => Node.Controller.Channel;

        public IZwaveNode Node { get; }
        public CommandClass Class { get; }

        protected internal virtual void HandleEvent(Command command)
        {
        }
    }
}
