using Core.Logging;

namespace App.Parsing.Handlers.Base
{
	public abstract class CommandLinePairHandler : CommandLineArgumentHandler, ICommandLineArgumentHandler
	{
		protected CommandLinePairHandler(string name) : base(name) { }

		public bool TryHandle(string[] arguments, ref int index)
		{
			var pair = arguments[index];

			if (!pair.StartsWith(Name))
				return false;

			Execute(pair.Split('=')[1].Trim());
			return true;
		}

		protected virtual void Execute(string value) => Debug.Log($"{Name} {value}");

		public virtual void ExecuteForce()
		{
			throw new System.NotImplementedException();
		}
	}
}