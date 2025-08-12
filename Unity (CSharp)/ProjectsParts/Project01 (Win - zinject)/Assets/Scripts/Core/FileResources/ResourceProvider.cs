using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.FileResources.Command;
using Core.FileResources.Command.Factory;
using Core.FileResources.Info;
using Cysharp.Threading.Tasks;

namespace Core.FileResources
{
	public class ResourceProvider : IResourceProvider, IDisposable
	{
		private readonly IResourceRequestCommandFactory _factory;
		private readonly IList<IResourceRequestCommand> _commands;

		public ResourceProvider(IResourceRequestCommandFactory factory)
		{
			_factory = factory;
			_commands = new List<IResourceRequestCommand>();
		}

		public void Dispose()
		{
			for (var i = _commands.Count - 1; i >= 0; i--)
				_commands[i].Cancel();

			_commands.Clear();
		}

		//public IResourceRequestCommand Request<TResource>(in ResourceInfo info,
		//	Action<TResource, string> onCompleted = null,
		//	Action<string> onFailure = null,
		//	Action onCanceled = null)
		//{
		//	return ExecuteCommand(in info, onCompleted, onFailure, onCanceled);
		//}

		public async UniTask<ResourceRequestResult<TResource>> RequestAsync<TResource>(ResourceInfo info,
			CancellationTokenSource cancellationTokenSource = null)
		{
			TResource resource = default;
			string filePath = null;
			string errorMessage = null;

			IResourceRequestCommand command = _commands.Count > 0
			? _commands.Where(x => x.Url == info.Url).First()
			: null;

			//IResourceRequestCommand command = (from p in _commands
			//																	 where p.Url == info.Url
			//																	 select p).First();

			if (command == null)
				command = ExecuteCommand<TResource>(in info,
				(r, fp) =>
				{
					resource = (TResource) r;
					filePath = fp;
				},
				em => errorMessage = em,
				null,
				cancellationTokenSource);

			if (command == null)
				return default;

			await UniTask.WaitWhile(() => command.InProgress);
			resource = (TResource) command.Resources;
			filePath = command.FilePath;

			return new ResourceRequestResult<TResource>(resource, filePath, errorMessage);
		}

		private IResourceRequestCommand ExecuteCommand<TResource>(in ResourceInfo info,
			Action<TResource, string> onCompleted,
			Action<string> onFailure,
			Action onCanceled,
			CancellationTokenSource cancellationTokenSource = null)
		{
			var command = _factory.Create<TResource>();

			if (command == null)
			{
				onFailure?.Invoke($"An error occurred while trying to create a resource request command for resource {info}");
				return null;
			}

			_commands.Add(command);

			command.Disposed += OnRequestDisposed;

			command.Initialize(in info, onCompleted, onFailure, onCanceled, cancellationTokenSource);
			command.Execute();

			return command;
		}

		private void OnRequestDisposed<TResource>(ResourceRequestCommand<TResource> command)
		{
			command.Disposed -= OnRequestDisposed;

			_ = _commands.Remove(command);
			_factory.Remove(command);
		}
	}
}