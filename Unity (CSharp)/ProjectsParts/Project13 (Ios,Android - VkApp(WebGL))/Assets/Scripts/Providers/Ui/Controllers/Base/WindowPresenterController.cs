using System;
using Cysharp.Threading.Tasks;
using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Presenters.Base;
using Game.Providers.Ui.Presenters.Factorys;
using Game.Providers.Ui.Settings;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Controllers.Base
{
	public abstract class WindowPresenterController<TPresenter> : WindowPresenterControllerBase, IWindowPresenterController
	where TPresenter : WindowPresenter
	{
		private IWindowPresenterFactory _presenterFactory;
		protected ISceneCanvas _sceneObjects;

		public TPresenter Presenter { get; private set; }
		public IWindowPresenter WindowPresenter => Presenter;

		[Inject]
		private void Initialize(IWindowPresenterFactory presenterFactory, ISceneCanvas sceneObjects)
		{
			_presenterFactory = presenterFactory;
			_sceneObjects = sceneObjects;
		}

		public override async UniTask<bool> Show(IWindowPresenterController source)
		{
			if (!await base.Show(source))
				return false;

			if (Presenter == null)
				Presenter = await Preload();

			if (Presenter == null)
				throw new NullReferenceException("No Exists prefab");

			Presenter.gameObject.SetActive(true);
			Presenter.Show();
			Presenter.transform.SetAsLastSibling();

			return true;
		}

		public override async UniTask<bool> Hide()
		{
			if (!await base.Hide())
				return false;

			if (Presenter == null)
				return false;

			Presenter.Hide();
			Presenter.gameObject.SetActive(false);

			return true;
		}

		private async UniTask<TPresenter> Preload()
		{
			Presenter = await _presenterFactory.GetInstance<TPresenter>();

			Presenter.gameObject.SetActive(false);

			Presenter.transform.SetParent(GetParent());

			RectTransform rectTransform = Presenter.transform as RectTransform;
			rectTransform.SetAsLastSibling();
			rectTransform.localScale = Vector3.one;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;

			_ = await Presenter.Initialize();

			AfterCreateWindow();

			return Presenter as TPresenter;
		}

		private Transform GetParent()
		{
			var attribute = this.GetType().GetAttribute<UiControllerAttribute>();

			var presenterType = attribute != null ? attribute.PresenterType : WindowPresenterType.Window;

			return presenterType switch
			{
				WindowPresenterType.Splash => _sceneObjects.SplashParent,
				WindowPresenterType.Popup => _sceneObjects.PopupsParent,
				WindowPresenterType.Window or _ => _sceneObjects.WindowsParent
			};
		}

		protected virtual void AfterCreateWindow() { }
	}
}
