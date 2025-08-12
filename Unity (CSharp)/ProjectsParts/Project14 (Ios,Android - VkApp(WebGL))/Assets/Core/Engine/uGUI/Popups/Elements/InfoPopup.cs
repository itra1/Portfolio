using Core.Engine.App.Base.Attributes;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Engine.uGUI.Popups
{
	[PrefabName(PopupTypes.Info)]
	public class InfoPopup : Popup
	{
		[SerializeField] private TMPro.TMP_Text _infoLabel;

		public float TimeLive { get; set; } = 1000;

		public void SetInfo(string text)
		{
			_infoLabel.text = text;
		}

		protected override void BeforeShow()
		{
			base.BeforeShow();
		}

		protected override void AfterShow()
		{
			base.AfterShow();
			DelayVisible().Forget();
		}

		private async UniTaskVoid DelayVisible()
		{
			await UniTask.Delay(2000);
			_ = Hide();
		}
	}
}
