using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace it.Popups
{
	public class ConfirmPopup : PopupBase
	{
		public UnityEngine.Events.UnityAction OnConfirm;
		public UnityEngine.Events.UnityAction OnCancel;

		[SerializeField] private TMPro.TextMeshProUGUI _descriptionLabel;

		public void SetDescriptionString(string desc){
			_descriptionLabel.text = desc;
			_descriptionLabel.gameObject.SetActive(true);
		}

		public void SetDescriptionLocalizeTerm(string term)
		{
			_descriptionLabel.text = I2.Loc.LocalizationManager.GetTranslation(term);
			_descriptionLabel.gameObject.SetActive(true);
		}

		public void ConfirmTouch()
		{
			OnConfirm?.Invoke();
			Hide();
		}
		public void CancelTouch()
		{
			OnCancel?.Invoke();
			Hide();
		}
	}
}