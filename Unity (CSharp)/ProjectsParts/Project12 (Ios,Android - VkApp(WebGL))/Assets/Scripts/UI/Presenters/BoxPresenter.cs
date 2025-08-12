using Cysharp.Threading.Tasks;
using Game.Scripts.UI.Presenters.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Presenters
{
	public class BoxPresenter : WindowPresenter
	{
		[SerializeField] private Button _closeButton;
		[SerializeField] private Button _receiveButton;

		public override async UniTask<bool> Initialize()
		{
			if (!await base.Initialize())
				return false;

			_closeButton.onClick.AddListener(CloseButtonTouch);
			_receiveButton.onClick.AddListener(ReceiveButtonTouch);

			return true;
		}

		private void ReceiveButtonTouch()
		{

		}

		private void CloseButtonTouch()
		{

		}
	}
}
