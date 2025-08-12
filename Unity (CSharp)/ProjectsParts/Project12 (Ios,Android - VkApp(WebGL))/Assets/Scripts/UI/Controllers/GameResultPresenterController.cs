using Engine.Scripts.Managers;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.GameResult)]
	public class GameResultPresenterController : WindowPresenterController<GameResultPresenter>
	{
		public UnityAction OnRepeatEvent;
		public UnityAction OnContinueEvent;

		private IGameHandler _gameHelper;
		private IUiNavigator _uiNavigator;
		public override bool AddInNavigationStack => false;

		[Inject]
		public void Build(IGameHandler gameHelper, IUiNavigator uiHelper)
		{
			_gameHelper = gameHelper;
			_uiNavigator = uiHelper;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();

			Presenter.OnRetryEvent.AddListener(RetryEvent);
			Presenter.OnContinueEvent.AddListener(ContinueEvent);
		}

		//public override UniTask<bool> Open(bool clearNavigationStack, bool addInNavigationStack)
		//{
		//	_ = _uiNavigator.GetController<GamePlayPresenterController>().Close();

		//	return base.Open(clearNavigationStack, addInNavigationStack);
		//}

		private void RetryEvent()
		{
			OnRepeatEvent?.Invoke();
			//_gameHelper.RestartSong();
			//_ = _uiNavigator.BackNavigation();
			//_ = _uiNavigator.GetController<GamePlayPresenterController>().Open(false);
			//_ = Close();
		}

		private void ContinueEvent()
		{
			OnContinueEvent?.Invoke();
			//_gameHelper.EndSong();
			//_ = Close();
			//_ = _uiHelper.GetController<HomePresenterController>().Open(null);
		}
	}
}
