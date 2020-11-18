using System.IO;
using System.IO.Ports;

namespace ZWave.Channel
{
    public class SerialPort : ISerialPort
    {
        private readonly System.IO.Ports.SerialPort _port;

        public SerialPort(string name)
        {
            _port = new System.IO.Ports.SerialPort(name, 115200, Parity.None, 8, StopBits.One);
        }

        public Stream InputStream => _port.BaseStream;

        public Stream OutputStream => _port.BaseStream;

        public void Open()
        {
            _port.Open();
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
        }

        public void Close()
        {
            _port.Close();
        }
    }
}
