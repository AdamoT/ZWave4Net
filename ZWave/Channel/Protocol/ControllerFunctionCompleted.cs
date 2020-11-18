namespace ZWave.Channel.Protocol
{
    internal class ControllerFunctionCompleted : ControllerFunctionMessage
    {
        public ControllerFunctionCompleted(Function function, byte[] payload)
            : base(MessageType.Response, function, payload)
        {
        }
    }
}
