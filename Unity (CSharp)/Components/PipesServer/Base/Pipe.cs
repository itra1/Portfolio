using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Environment.Microsoft.Windows.Apps.Office.Server.IO;
using Pipes.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Pipes.Base
{
	public abstract class Pipe : IPipe
	{
		private const string KetLog = "[PIPE]";
		public UnityEvent OnConnected { get; set; } = new();
		public UnityEvent OnPipeIsBroken { get; set; } = new();

		protected IStringStream _stringStream;
		protected Encoding _streamEncoding;

		private readonly ConcurrentQueue<IPackageData> _requests;

		protected readonly CancellationTokenSource _disposeCancellationTokenSource;
		protected CancellationTokenSource _disconnectCancellationTokenSource;

		public bool Connected { get; protected set; }
		public bool LockedDisposeInFactory { get; set; } = false;

		protected abstract void Close();

		protected Pipe()
		{
			_disposeCancellationTokenSource = new CancellationTokenSource();
			_requests = new();
		}

		public abstract void Create(string serverName);
		public abstract UniTask<bool> ConnectAsync();
		public abstract UniTask<bool> DisconnectAsync();

		protected void SetStreamString(Stream stream)
		{
			_stringStream = new StringStream(stream, new UTF8Encoding());
		}

		protected async UniTask RequestQueueProcess()
		{
			CancellationTokenSource mergedCancellationTokenSource = null;

			try
			{
				mergedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposeCancellationTokenSource.Token,
						_disconnectCancellationTokenSource.Token);

				var cancellationToken = mergedCancellationTokenSource.Token;

				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();

					while (Connected)
					{
						if (!_requests.TryDequeue(out var request))
						{
							await UniTask.Yield(cancellationToken: cancellationToken);
							continue;
						}

						try
						{
							await Send(request);
						}
						catch (System.Exception ex)
						{
							Debug.LogError($"{KetLog} {ex.Message}");
							OnPipeIsBroken?.Invoke();
						}
						finally
						{
						}
					}
				}
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				UnityEngine.Debug.LogException(exception);
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();

				mergedCancellationTokenSource?.Dispose();
			}
		}

		public async UniTask<IPackageData> Request(IPackageData package)
		{
			_requests.Enqueue(package);
			await UniTask.WaitUntil(() => package.Completed);
			return package;
		}

		/// <summary>
		/// Sending a single message
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		protected async UniTask Send(IPackageData package)
		{
			_stringStream.Write(package.OutgoingPacket);

			await UniTask.WaitUntil(() => _stringStream.CanRead);
			package.IncomingPacket = _stringStream.Read();

			package.Completed = true;
		}

		public void Dispose()
		{
			_ = DisconnectAsync();
			if (_disposeCancellationTokenSource is { IsCancellationRequested: false })
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
		}
	}
}
