using Game.Scripts.App;

namespace Game.Scripts.Providers.Premiums
{
	public interface IPremiumProvider : IApplicationLoaderItem
	{
		bool IsActive { get; }
	}
}