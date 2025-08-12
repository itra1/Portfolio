using System.IO;
using System.Text;

namespace OfficeControl.Pipes.Base
{
	public class StreamString
	{
		private Stream _ioStream;
		private Encoding _encoding;

		public StreamString(Stream ioStream, Encoding encoding)
		{
			this._ioStream = ioStream;
			_encoding = encoding;
		}

		public string ReadString()
		{
			var len = _ioStream.ReadByte() * 256;
			len += _ioStream.ReadByte();
			byte[] inBuffer = new byte[len];
			_ = _ioStream.Read(inBuffer, 0, len);

			return _encoding.GetString(inBuffer);
		}

		public int WriteString(string outString)
		{
			byte[] outBuffer = _encoding.GetBytes(outString);
			int len = outBuffer.Length;
			if (len > UInt16.MaxValue)
			{
				len = (int)UInt16.MaxValue;
			}
			_ioStream.WriteByte((byte)(len / 256));
			_ioStream.WriteByte((byte)(len & 255));
			_ioStream.Write(outBuffer, 0, len);
			_ioStream.Flush();

			return outBuffer.Length + 2;
		}
	}
}
