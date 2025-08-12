using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using OfficeControl.Pipes.Base;

namespace OfficeControl.Controllers
{
	internal class PipeClient
	{
		public static PipeClient Instance { get; private set; }
		private NamedPipeClientStream _pipeClient;
		private StreamString _streamingStream;
		private Encoding _streamEncoding;
		private string _serverKey;
		private bool _isConnected;

		public PipeClient(string serverKey)
		{
			Instance = this;
			_streamEncoding = new UTF8Encoding();
			_serverKey = serverKey;
			_pipeClient = new NamedPipeClientStream(".", _serverKey,
														PipeDirection.InOut, PipeOptions.None,
														TokenImpersonationLevel.Impersonation);
			_streamingStream = new StreamString(_pipeClient, _streamEncoding);

			RunClient();
		}

		private async void RunClient()
		{

			Console.WriteLine("Connecting to server...\n");

			try
			{
				_pipeClient.Connect(10000);
				Console.WriteLine("Connected server\n");

				_isConnected = true;

				while (_isConnected)
				{
					string request = _streamingStream.ReadString();

					Console.WriteLine("Request " + request);
					try
					{
						string answer = await PackageManager.Instance.ProcessMessage(request);

						Console.WriteLine("Answer " + request);
						_ = _streamingStream.WriteString(answer);
					}
					catch
					{
						string answer = await PackageManager.Instance.MakeErrorMessage();

						Console.WriteLine("Answer " + request);
						_ = _streamingStream.WriteString(answer);
					}
				}
			}
			catch (IOException ex)
			{
				Console.WriteLine("Server disconnect...\n");
				Console.WriteLine("Error " + ex.Message + " " + ex.StackTrace);
				_isConnected = false;
				RunClient();
				return;
			}
			catch (TimeoutException)
			{
				Console.Write($"timeout close");
			}
			catch (System.Exception ex)
			{
				Console.Write($"Ошибка {ex.Message} {ex.StackTrace}");
			}
			Console.Write($"client close");
			_pipeClient.Close();
			Environment.Exit(0);
		}
	}
}
