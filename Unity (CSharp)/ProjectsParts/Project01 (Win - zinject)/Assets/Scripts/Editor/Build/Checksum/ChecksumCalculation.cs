using System;
using System.IO;
using System.Security.Cryptography;

namespace Editor.Build.Checksum
{
	public class ChecksumCalculation : IChecksum
	{
		public string Calculate(string path)
		{
			using var fileStream = File.OpenRead(path);
			MD5 md5 = new MD5CryptoServiceProvider();
			var bytes = new byte[fileStream.Length];
			fileStream.Read(bytes, 0, (int) fileStream.Length);
			var checkSum = md5.ComputeHash(bytes);
			var result = BitConverter.ToString(checkSum).Replace("-", string.Empty);
			return result;
		}
	}
}