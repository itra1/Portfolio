using Cysharp.Threading.Tasks;

using Providers.Splash.Presenter;

using System;

namespace Providers.Splash
{
	public class SplashProvider : ISplashProvider
	{
		private ISplashPresenter _splashPresenter;
		public SplashProvider(ISplashPresenter splashPresenter)
		{
			_splashPresenter = splashPresenter;
		}

		public async UniTask Play()
		{
			bool isComplete = false;
			_splashPresenter.Play(() =>
			{
				isComplete = true;
			});

			await UniTask.WaitUntil(() => isComplete);



		}
	}
}
