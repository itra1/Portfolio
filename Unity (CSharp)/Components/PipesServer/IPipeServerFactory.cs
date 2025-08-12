using Pipes;

namespace Components.Pipes
{
	public interface IPipeServerFactory
	{
		IPipeServer Create();
	}
}