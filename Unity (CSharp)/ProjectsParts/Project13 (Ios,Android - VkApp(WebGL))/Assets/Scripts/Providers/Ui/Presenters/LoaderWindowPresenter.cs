using Game.Base.AppLaoder;
using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Presenters.Base;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Presenters
{
	public class LoaderWindowPresenter : WindowPresenter
	{
		[SerializeField] private ProgressView _loader;

		private IAppLoaderHandler _loadingHandler;

		[Inject]
		public void Constructor(IAppLoaderHandler loadingHandler)
		{
			_loadingHandler = loadingHandler;
		}

		public override void Show()
		{
			base.Show();
			SetProgressValue(0);
			_loadingHandler.OnProgress.AddListener(SetProgressValue);
		}

		public override void Hide()
		{
			_loadingHandler.OnProgress.RemoveListener(SetProgressValue);
			base.Hide();
		}

		private void SetProgressValue(float value)
		{
			_loader.SetValue(value, $"{(int) (value * 100)}%");
		}
	}
}
