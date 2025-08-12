

using UnityEngine.Events;

namespace Providers.Splash.Presenter
{
	public interface ISplashPresenter
	{
		void Play(UnityAction onComplete);
	}
}
