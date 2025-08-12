using System.IO;
using System.Net;
using DG.DemiEditor;
using FTP.Parsers;
using FTP.Structs;
using UnityEngine;

namespace Ftp
{
	public class FtpClient
	{
		private string _host;
		private string _userName;
		private string _password;
		private bool _useSSL = true;

		public FtpClient(string host, string userName, string password, bool useSsl = false)
		{
			_host = host;
			_userName = userName;
			_password = password;
			_useSSL = useSsl;
		}

		private FtpWebRequest MakeRequest(string path = null)
		{
			if (path.IsNullOrEmpty())
			{
				path = "/";
			}

			var ftpRequest = (FtpWebRequest) WebRequest.Create("ftp://" + _host + path);
			ftpRequest.Credentials = new NetworkCredential(_userName, _password);
			ftpRequest.EnableSsl = _useSSL;
			return ftpRequest;
		}

		public FileStruct[] ListDirectory(string path = null)
		{
			var ftpRequest = MakeRequest(path);

			ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			var ftpResponse = (FtpWebResponse) ftpRequest.GetResponse();

			StreamReader sr = new StreamReader(ftpResponse.GetResponseStream(), System.Text.Encoding.ASCII);

			var content = sr.ReadToEnd();
			sr.Close();

			ftpResponse.Close();

			DirectoryListParser parser = new DirectoryListParser(content);
			return parser.FullListing;
		}

		public void DownloadFile(string path, string fileName)
		{
			var ftpRequest = MakeRequest(path);
			ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

			FileStream downloadedFile = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);

			var ftpResponse = (FtpWebResponse) ftpRequest.GetResponse();
			Stream responseStream = ftpResponse.GetResponseStream();

			byte[] buffer = new byte[1024];
			int size;
			while ((size = responseStream.Read(buffer, 0, 1024)) > 0)
			{
				downloadedFile.Write(buffer, 0, size);
			}

			ftpResponse.Close();
			downloadedFile.Close();
			responseStream.Close();
		}

		public void UploadFile(string path, string fileName)
		{
			Debug.Log($"UploadFile {path}");
			_ = fileName.Remove(0, fileName.LastIndexOf("\\") + 1);

			FileStream uploadedFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);

			var ftpRequest = MakeRequest(path);
			ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

			byte[] file_to_bytes = new byte[uploadedFile.Length];

			_ = uploadedFile.Read(file_to_bytes, 0, file_to_bytes.Length);

			uploadedFile.Close();

			Stream writer = ftpRequest.GetRequestStream();

			writer.Write(file_to_bytes, 0, file_to_bytes.Length);
			writer.Close();
		}

		public void DeleteFile(string path)
		{
			var ftpRequest = MakeRequest(path);
			ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;

			FtpWebResponse ftpResponse = (FtpWebResponse) ftpRequest.GetResponse();
			ftpResponse.Close();
		}

		public void CreateDirectory(string path)
		{
			Debug.Log($"CreateDirectory {path}");
			FtpWebRequest ftpRequest = MakeRequest(path);
			ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;

			FtpWebResponse ftpResponse = (FtpWebResponse) ftpRequest.GetResponse();
			ftpResponse.Close();
		}

		public void RemoveDirectory(string path)
		{
			FtpWebRequest ftpRequest = MakeRequest(path);
			ftpRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;

			FtpWebResponse ftpResponse = (FtpWebResponse) ftpRequest.GetResponse();
			ftpResponse.Close();
		}
	}
}