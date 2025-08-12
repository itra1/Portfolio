using Game.Base.AppLaoder;

namespace Game.Providers.Nicknames
{
	public interface INicknamesProvider : IAppLoaderElement
	{
		string GetRandom();
	}
}