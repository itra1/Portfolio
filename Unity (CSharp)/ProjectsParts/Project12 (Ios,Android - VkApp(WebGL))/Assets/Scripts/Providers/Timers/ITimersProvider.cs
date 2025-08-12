using Game.Scripts.Providers.Timers.Common;

namespace Game.Scripts.Providers.Timers
{
	internal interface ITimersProvider
	{
		ITimer Create(string type);
	}
}