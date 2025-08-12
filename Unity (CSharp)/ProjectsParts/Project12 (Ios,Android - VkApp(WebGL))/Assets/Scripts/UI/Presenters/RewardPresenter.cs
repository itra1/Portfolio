using Game.Scripts.UI.Presenters.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Presenters
{
	public class RewardPresenter : WindowPresenter
	{
		[SerializeField] private Button _closeButton;
		[SerializeField] private Button _claimButton;

		[Inject]
		private void Constructor()
		{
			_closeButton.onClick.RemoveAllListeners();
			_closeButton.onClick.AddListener(CloseButtonTouch);

			_claimButton.onClick.RemoveAllListeners();
			_claimButton.onClick.AddListener(ClaimButtonTouch);
		}

		private void CloseButtonTouch()
		{
		}

		private void ClaimButtonTouch()
		{
		}
	}
}
