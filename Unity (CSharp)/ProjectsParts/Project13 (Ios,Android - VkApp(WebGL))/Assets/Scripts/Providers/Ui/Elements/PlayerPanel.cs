using Cysharp.Threading.Tasks;
using Game.Base;
using Game.Providers.Ui.Controllers;
using Game.Providers.Ui.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class PlayerPanel : MonoBehaviour, IInjection
	{
		[SerializeField] private Button _selfButton;

		private WindowProvider _windowProvider;
		private IUiProvider _uiProvider;

		[Inject]
		public void Constructor(WindowProvider windowProvider, IUiProvider uiProvider)
		{
			_windowProvider = windowProvider;
			_uiProvider = uiProvider;

			_selfButton.onClick.AddListener(SelfClickTouch);
		}

		private void SelfClickTouch()
		{
			var activeWindow = _windowProvider.GetActiveWindow();
			activeWindow.Hide().Forget();
			var profileWindow = _uiProvider.GetController<ProfileWindowPresenterController>();
			//var profileWindow = _windowProvider.GetWindow(WindowsNames.Profile);
			profileWindow.Show(null).Forget();
			//_ = profileWindow.OnHide(() =>
			//{
			//	activeWindow.Show().Forget();
			//});
		}
	}
}
