using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Core.Base;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.States;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Core.Network.Socket.ActionTypes
{
	public class SocketActionTypesInfo : ISocketActionPackets, ILateInitialized, IDisposable
	{
		private readonly CancellationTokenSource _disposeCancellationTokenSource;
		
		public bool IsInitialized { get; private set; }
		
		public IReadOnlyDictionary<string, SocketActionTypeData> PacketTypes { get; private set; }
		
		public SocketActionTypesInfo()
		{
			_disposeCancellationTokenSource = new CancellationTokenSource();
			CollectTypesAsync().Forget();
		}
		
		public void Dispose()
		{
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
			
			PacketTypes = null;
		}
		
		private async UniTaskVoid CollectTypesAsync()
		{
			try
			{
				if (!Thread.CurrentThread.IsBackground && Application.isPlaying)
				{
					var cancellationToken = _disposeCancellationTokenSource.Token;
					
					await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
					{
						await UniTask.SwitchToThreadPool();
						cancellationToken.ThrowIfCancellationRequested();
						CollectTypes();
					}
				}
				else
				{
					CollectTypes();
				}
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
				
				IsInitialized = true;
			}
		}

		private void CollectTypes()
		{
			PacketTypes = CollectTypes<IncomingAction, SocketActionAttribute>();
		}
		
		private IReadOnlyDictionary<string, SocketActionTypeData> CollectTypes<TAction, TActionAttribute>() 
			where TActionAttribute : SocketPacketAttribute
		{
			var packets = new Dictionary<string, SocketActionTypeData>();
			
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsSubclassOf(typeof(TAction)))
					continue;
				
				foreach (var attribute in type.GetCustomAttributes<TActionAttribute>(false)) 
					packets.Add(attribute.PacketName, new SocketActionTypeData(type, attribute.ReplaceName));
			}
			
			return packets;
		}
	}
}