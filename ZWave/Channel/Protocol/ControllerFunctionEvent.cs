namespace ZWave.Channel.Protocol
{
    internal class ControllerFunctionEvent : ControllerFunctionMessage
    {
        public ControllerFunctionEvent(Function function, byte[] payload)
            : base(MessageType.Request, function, payload)
        {
        }
    }
}
