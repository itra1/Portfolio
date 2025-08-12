using Cysharp.Threading.Tasks;
using Game.Scripts.UI.Presenters.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.UI.Presenters
{
	public class LevelUpPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent OnClaimTouch = new();

		[SerializeField] private Button _closeButton;
		[SerializeField] private Button _claimButton;
		[SerializeField] private TMP_Text _levelLabel;

		public override async UniTask<bool> Initialize()
		{
			if (!await base.Initialize())
				return false;

			_claimButton.onClick.RemoveAllListeners();
			_claimButton.onClick.AddListener(CloseButtonTouch);
			_closeButton.onClick.RemoveAllListeners();
			_closeButton.onClick.AddListener(CloseButtonTouch);

			return true;
		}

		public void SetLevel(int level)
		{
			_levelLabel.text = level.ToString();
		}

		private void CloseButtonTouch()
		{
			OnClaimTouch?.Invoke();
		}
	}
}
