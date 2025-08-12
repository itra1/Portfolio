using System;
using Cysharp.Threading.Tasks;
using Pipes.Data;
using UnityEngine.Events;

namespace Pipes.Base
{
	public interface IPipe : IDisposable
	{
		/// <summary>
		/// Immediate destruction blocked
		/// </summary>
		bool LockedDisposeInFactory { get; set; }

		/// <summary>
		/// Successful connection event
		/// </summary>
		UnityEvent OnConnected { get; set; }
		UnityEvent OnPipeIsBroken { get; set; }

		bool Connected { get; }
		/// <summary>
		/// Create a thread instance
		/// </summary>
		/// <param name="serverName"></param>
		void Create(string serverName);
		/// <summary>
		/// Making the connection
		/// </summary>
		/// <param name="serverName"></param>
		/// <returns></returns>
		UniTask<bool> ConnectAsync();

		/// <summary>
		/// Performing a disconnect
		/// </summary>
		/// <returns></returns>
		UniTask<bool> DisconnectAsync();

		UniTask<IPackageData> Request(IPackageData package);

	}
}
