using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Factory;
using Cysharp.Threading.Tasks;

namespace App.Parsing
{
	public class CommandLineArgumentsParser : ICommandLineArgumentsParser
	{
		private readonly ICommandLineArgumentHandlerFactory _factory;
		private ISet<ICommandLineArgumentHandler> _handlers;

		public CommandLineArgumentsParser(ICommandLineArgumentHandlerFactory factory) => _factory = factory;

		public async UniTask ParseAsync(CancellationToken cancellationToken)
		{
			try
			{
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();

					cancellationToken.ThrowIfCancellationRequested();

					_handlers = CollectHandlers();
					var arguments = System.Environment.GetCommandLineArgs();

					var index = 0;

					while (index < arguments.Length)
					{
						foreach (var handler in _handlers)
						{
							cancellationToken.ThrowIfCancellationRequested();

							if (!handler.TryHandle(arguments, ref index))
								continue;

							_ = _handlers.Remove(handler);
							break;
						}

						index++;
					}
				}
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
			}
		}

		public void Handle(Type handle)
		{
			foreach (var handler in _handlers)
			{
				if (handler.GetType() != handle)
					continue;

				handler.ExecuteForce();

				_ = _handlers.Remove(handler);
				break;
			}
		}

		private ISet<ICommandLineArgumentHandler> CollectHandlers()
		{
			var handlers = new HashSet<ICommandLineArgumentHandler>();

			var handlerTypeBase = typeof(ICommandLineArgumentHandler);

			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsClass || type.IsAbstract || !handlerTypeBase.IsAssignableFrom(type))
					continue;

				_ = handlers.Add(_factory.Create(type));
			}

			return handlers;
		}
	}
}