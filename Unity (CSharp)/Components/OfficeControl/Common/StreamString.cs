using System;
using System.IO;
using System.Text;
namespace OfficeControl
{
	public class StreamString
	{
		private readonly Stream _ioStream;
		private readonly UnicodeEncoding _streamEncoding;

		public StreamString(Stream ioStream)
		{
			this._ioStream = ioStream;
			_streamEncoding = new UnicodeEncoding();
		}

		public string ReadString()
		{
			try
			{
				var len = _ioStream.ReadByte() * 256;
				len += _ioStream.ReadByte();
				var inBuffer = new byte[len];
				_ioStream.Read(inBuffer, 0, len);

				return _streamEncoding.GetString(inBuffer);
			}
			catch
			{
				return "";
			}
		}

		public int WriteString(string outString)
		{
			var outBuffer = _streamEncoding.GetBytes(outString);
			var len = outBuffer.Length;
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