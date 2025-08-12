using Game.Assets.Scripts.Providers.Platforms.Base;

namespace Game.Scripts.Providers.Platforms
{
	public interface IPlatformsProvider
	{
		IPlatformActions PlatformActions { get; }
	}
}