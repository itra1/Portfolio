using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Presenters.Base;
using UnityEngine.Events;

namespace Game.Providers.Ui.Presenters
{
	[UiController(WindowPresenterType.Popup, WindowPresenterNames.Welcome)]
	public class WelcomeWindowPresenter : WindowPresenter
	{
		public UnityAction NoTutorialAction;
		public UnityAction RunTutorialAction;

		public void NoButtonTouch()
		{
			NoTutorialAction?.Invoke();
			Hide();
		}

		public void YesButtonTouch()
		{
			RunTutorialAction?.Invoke();
			Hide();
		}
	}
}
