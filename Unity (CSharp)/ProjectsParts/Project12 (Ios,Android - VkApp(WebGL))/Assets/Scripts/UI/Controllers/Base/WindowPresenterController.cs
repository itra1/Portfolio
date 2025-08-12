using System;
using Cysharp.Threading.Tasks;
using Engine.Scripts.Common.Interfaces;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Presenters.Base;
using Game.Scripts.UI.Presenters.Factorys;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Controllers.Base
{
	public abstract class WindowPresenterController<TPresenter> : WindowPresenterControllerBase, IWindowPresenterController
	where TPresenter : WindowPresenter
	{
		private IWindowPresenterFactory _presenterFactory;

		protected ISceneUiParents _sceneUiParents;

		public abstract bool AddInNavigationStack { get; }
		protected TPresenter Presenter { get; private set; }
		protected IUiNavigator UiNavigator { get; private set; }
		public IWindowPresenter WindowPresenter => Presenter;

		[Inject]
		private void Initialize(IWindowPresenterFactory presenterFactory, ISceneUiParents sceneUiParents, IUiNavigator uiHelper)
		{
			_presenterFactory = presenterFactory;
			_sceneUiParents = sceneUiParents;
			UiNavigator = uiHelper;
		}

		public async UniTask LoadPresenter()
		{
			if (Presenter == null)
				Presenter = await Preload();
		}

		public override async UniTask<bool> Open()
		{
			if (!await base.Open())
				return false;

			await LoadPresenter();

			if (Presenter == null)
				throw new NullReferenceException("No Exists prefab");

			Presenter.transform.SetAsLastSibling();
			await Presenter.Show();
			IsOpen = true;
			EmitOnPresenterVisibleChange();

			return true;
		}

		public override async UniTask<bool> Close()
		{
			if (!await base.Close())
				return false;

			if (Presenter == null)
				return false;

			await Presenter.Hide();
			IsOpen = false;
			EmitOnPresenterVisibleChange();

			return true;
		}

		private async UniTask<TPresenter> Preload()
		{
			Presenter = _presenterFactory.GetInstance<TPresenter>();

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
				WindowPresenterType.Splash => _sceneUiParents.SplashParent,
				WindowPresenterType.Popup => _sceneUiParents.PopupParent,
				WindowPresenterType.Window or _ => _sceneUiParents.WindowsParent
			};
		}

		protected virtual void AfterCreateWindow() { }
	}
}
