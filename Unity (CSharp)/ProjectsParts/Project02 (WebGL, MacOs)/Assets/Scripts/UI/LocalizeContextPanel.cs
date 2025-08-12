using UnityEngine;
using UnityEngine.UI;
using TMPro;
using I2.Loc;
using Sett = it.Settings;

namespace it.UI
{
	public class LocalizeContextPanel : ContextFooterPanel
	{
		[SerializeField] private Image _flag;
		[SerializeField] private TextMeshProUGUI _title;
		protected override void OnEnable()
		{
			base.OnEnable();

			//com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, OnAuthorization);
			I2.Loc.LocalizationManager.OnLocalizeEvent -= ConfirmPanel;
			I2.Loc.LocalizationManager.OnLocalizeEvent += ConfirmPanel;
		}

		private void OnAuthorization(com.ootii.Messages.IMessage handler)
		{
			ConfirmPanel();
		}

		private void ConfirmPanel()
		{
			//if (UserController.User == null) return;
			var lng = Sett.AppSettings.Languages.Find(x => x.Code == I2.Loc.LocalizationManager.CurrentLanguageCode);
			_title.text = lng.NativeName;
			_flag.sprite = lng.Flag;
		}
	}
}