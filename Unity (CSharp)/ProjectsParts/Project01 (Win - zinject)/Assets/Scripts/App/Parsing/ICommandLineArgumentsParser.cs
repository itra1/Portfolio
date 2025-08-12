using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Parsing
{
	public interface ICommandLineArgumentsParser
	{
		UniTask ParseAsync(CancellationToken cancellationToken);
		void Handle(Type handle);

	}
}