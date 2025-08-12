namespace App.Parsing.Handlers.Base
{
	public interface ICommandLineArgumentHandler
	{
		bool TryHandle(string[] arguments, ref int index);
		void ExecuteForce();
	}
}