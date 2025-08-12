using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Sett = it.Settings;
using Garilla;

namespace it.UI
{
	public class HelpPanel : ContextFooterPanel
	{
		[SerializeField] private TextMeshProUGUI _email;


		protected override void OnEnable()
		{
			base.OnEnable();
			_email.text = Sett.AppSettings.EmailSupport;
		}

		public void EmailButton()
		{
			Application.OpenURL("mailto:" + Sett.AppSettings.EmailSupport);
		}

		public void TermsButton()
		{
			LinkManager.OpenUrl("termAndConditions");
		}

		public void SecurityButton()
		{
			LinkManager.OpenUrl("privacyPolicy");
		}

		public void LiveChatButton()
		{
			LinkManager.OpenUrl("liveChat");
		}
	}
}