using System.Collections.Generic;
using Core.Logging;

namespace App.Parsing.Handlers.Base
{
	public abstract class CommandLineValuesHandler : CommandLineArgumentHandler, ICommandLineArgumentHandler
	{
		private readonly int _valuesCount;

		protected CommandLineValuesHandler(string name, int valuesCount) : base(name)
		{
			_valuesCount = valuesCount;
		}

		public void ExecuteForce()
		{
			throw new System.NotImplementedException();
		}

		public bool TryHandle(string[] arguments, ref int index)
		{
			if (Name != arguments[index])
				return false;

			if (_valuesCount > 0)
			{
				var values = new string[_valuesCount];

				for (var i = 0; i < _valuesCount; i++)
					values[i] = arguments[++index];

				Execute(values);
			}

			return true;
		}

		protected virtual void Execute(IReadOnlyList<string> values) =>
				Debug.Log($"{Name} {string.Join(',', values)}");
	}
}