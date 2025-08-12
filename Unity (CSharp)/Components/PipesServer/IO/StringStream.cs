using System.IO;
using System.Text;

namespace Environment.Microsoft.Windows.Apps.Office.Server.IO
{
    public class StringStream : IStringStream
    {
        private readonly Stream _stream;
        private readonly Encoding _encoding;

        public StringStream(Stream stream, Encoding encoding)
        {
            _stream = stream;
            _encoding = encoding;
        }

        public bool CanRead => _stream.CanRead;

        public string Read()
        {
            try
            {
                var buffer = new byte[_stream.ReadByte() * 256 + _stream.ReadByte()];

                return _stream.Read(buffer, 0, buffer.Length) > 0
                    ? _encoding.GetString(buffer)
                    : null;
            }
            finally
            {
                _stream.Flush();
            }
        }

        public void Write(string value)
        {
            try
            {
                var buffer = _encoding.GetBytes(value);

                var length = buffer.Length;

                if (length > ushort.MaxValue)
                    length = ushort.MaxValue;

                byte[] sendBuffer = new byte[buffer.Length + 2];

                sendBuffer[0] = (byte) (length / 256);
                sendBuffer[1] = (byte) (length & 255);

                for (int i = 0; i < buffer.Length; i++)
                    sendBuffer[i + 2] = buffer[i];

                _stream.Write(sendBuffer, 0, sendBuffer.Length);
            }
            finally
            {
                _stream.Flush();
            }
        }
    }
}