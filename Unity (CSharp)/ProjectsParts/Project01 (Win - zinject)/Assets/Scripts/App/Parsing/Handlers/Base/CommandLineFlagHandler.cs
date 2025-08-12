using Core.Logging;

namespace App.Parsing.Handlers.Base
{
	public abstract class CommandLineFlagHandler : CommandLineArgumentHandler, ICommandLineArgumentHandler
	{
		protected CommandLineFlagHandler(string name) : base(name) { }

		public virtual void ExecuteForce()
		{
			throw new System.NotImplementedException();
		}

		public virtual bool TryHandle(string[] arguments, ref int index)
		{
			if (Name != arguments[index])
				return false;

			Execute();
			return true;
		}

		protected virtual void Execute() => Debug.Log($"{Name}");
	}
}