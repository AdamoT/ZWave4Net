using System;

namespace ZWave
{
    public class ErrorEventArgs : EventArgs
    {
        public readonly Exception Error;

        public ErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
