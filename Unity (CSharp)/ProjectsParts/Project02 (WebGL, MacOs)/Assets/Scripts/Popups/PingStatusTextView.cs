using UnityEngine;
using UnityEngine.UI;

namespace it.Popups
{
	public class PingStatusTextView : PingViewBase
	{
		[SerializeField] private TMPro.TextMeshProUGUI _label;
		[SerializeField] private Color _pendingColor;
		[SerializeField] private Color _normalColor;
		[SerializeField] private Color _fastColor;
		[SerializeField] private Color _lostColor;

		private GameObject currentText;

		private void OnEnable()
		{
			_label.gameObject.SetActive(true);
			ShowPendind();
		}

		private void ShowPendind()
		{
			_label.text = I2.Loc.LocalizationManager.GetTranslation("popup.network.status.pendind");
			_label.color = _pendingColor;
		}
		private void ShowNormal()
		{
			_label.text = I2.Loc.LocalizationManager.GetTranslation("popup.network.status.normal");
			_label.color = _normalColor;
		}
		private void ShowFast()
		{
			_label.text = I2.Loc.LocalizationManager.GetTranslation("popup.network.status.fast");
			_label.color = _fastColor;
		}
		private void ShowLost()
		{
			_label.text = I2.Loc.LocalizationManager.GetTranslation("popup.network.status.lose");
			_label.color = _lostColor;
		}

		public override void SetPing(long ping)
		{
			if (ping >= 300)
				ShowLost();

			if (ping >= 100 & ping < 300)
				ShowNormal();

			if (ping < 100)
				ShowFast();

		}
		public override void Clear()
		{
			ShowPendind();
		}
	}
}
