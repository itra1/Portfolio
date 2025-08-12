using Ftp;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FTP.Tests
{
	[TestFixture]
	public class FtpTests
	{
		private string host = "ftp.net1741037.nichost.ru";
		private string userName = "net1741037_gd";
		private string password = "5fgH53DG45";
		private bool useSsl = false;

		private FtpClient _client;

		[UnitySetUp]
		public void Setup()
		{
			_client = new FtpClient(host, userName, password, useSsl);
		}

		[Test]
		public void ReadDirsRoot()
		{
			Setup();
			ReadDir("/");
		}

		[Test]
		public void CreateDirRoot()
		{
			Setup();
			_client.CreateDirectory($"/{System.Guid.NewGuid()}");
			ReadDir("/");
		}

		[Test]
		public void RemoveDir()
		{
			Setup();
			//_client.RemoveDirectory($"/151dcb07-3142-492f-b64d-bbd1cef50cd8");
			_client.RemoveDirectory($"/9c9cbab5-b57f-48f8-ad3e-ae281170c53b");
			ReadDir("/");
		}

		private void ReadDir(string path)
		{
			var list = _client.ListDirectory(path);

			Debug.Log($"List count {list.Length}");

			foreach (var item in list)
			{
				Debug.Log($"{item.Name} if dir {item.IsDirectory}");
			}
		}
	}
}