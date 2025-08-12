using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Core.Logging;
using Cysharp.Threading.Tasks;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Attributes;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;
using Leguar.TotalJSON;
using Newtonsoft.Json;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets.Converting.Deserializers
{
	public class PacketJsonDeserializer : IPacketDeserializer
	{
		private readonly IDictionary<string, Type> _packetTypesByName;
		private readonly CancellationTokenSource _disposeCancellationTokenSource;

		public bool IsInitialized { get; private set; }

		public PacketJsonDeserializer()
		{
			_packetTypesByName = new Dictionary<string, Type>();
			_disposeCancellationTokenSource = new CancellationTokenSource();

			CollectPacketTypesAsync().Forget();
		}

		public IPacket Deserialize(string rawData)
		{
			var jArray = JArray.ParseString(rawData);

			var packetName = jArray.GetString(0);
			var packetValue = jArray.GetJSON(1).CreateString();

			return (IPacket) JsonConvert.DeserializeObject(packetValue, _packetTypesByName[packetName]);
		}

		public void Dispose()
		{
			if (_disposeCancellationTokenSource is { IsCancellationRequested: false })
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}

			_packetTypesByName.Clear();
		}

		private async UniTaskVoid CollectPacketTypesAsync()
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;

				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();

					cancellationToken.ThrowIfCancellationRequested();

					var packetTypeBase = typeof(PacketBase);

					foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
					{
						cancellationToken.ThrowIfCancellationRequested();

						if (!type.IsClass || !packetTypeBase.IsAssignableFrom(type))
							continue;

						var attribute = type.GetCustomAttribute<PacketNameAttribute>();

						if (attribute == null)
							continue;

						_packetTypesByName.Add(attribute.Name, type);
					}
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
	}
}