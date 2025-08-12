using System;
using System.Collections.Generic;
using Pipes;

namespace Components.Pipes
{
	public class PipeServerFactory : IPipeServerFactory, IDisposable
	{
		private List<IPipeServer> _serversList = new();

		public IPipeServer Create()
		{
			var server = new PipeServer();
			_serversList.Add(server);
			return server;
		}

		public void Dispose()
		{
			for (int i = 0; i < _serversList.Count; i++)
			{
				if (!_serversList[i].LockedDisposeInFactory)
					_serversList[i].Dispose();
			}
			_serversList = null;
		}
	}
}
