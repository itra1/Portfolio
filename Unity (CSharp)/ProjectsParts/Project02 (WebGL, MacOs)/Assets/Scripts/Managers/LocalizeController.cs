using System.Collections;
using UnityEngine;
using I2.Loc;
using com.ootii.Utilities.Debug;
using Sett = it.Settings;

public class LocalizeController : Singleton<LocalizeController>
{

	private void OnEnable()
	{
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, UserLogin);

		string lngCode = PlayerPrefs.GetString("LANGUAGE", "");

		if(!string.IsNullOrEmpty(lngCode)){
			SelLocale(lngCode);
			return;
		}

		switch ( Application.systemLanguage)
		{
			case SystemLanguage.English: I2.Loc.LocalizationManager.SetLanguageAndCode("English", "en"); break;
			case SystemLanguage.Russian: I2.Loc.LocalizationManager.SetLanguageAndCode("Russian", "ru"); break;
			case SystemLanguage.Unknown:
			default: I2.Loc.LocalizationManager.SetLanguageAndCode("English", "en"); break;
		}
	}

	private void UserLogin(com.ootii.Messages.IMessage handle)
	{
		if (UserController.User == null) return;
		var lng = UserController.User.user_profile.language;

		SelLocale(lng.slug.ToLower());
	}

	public void SelLocale(string code)
	{
		var setL = Sett.AppSettings.Languages.Find(x => x.Code == code);
		if (setL == null) return;

		if(UserController.Instance != null && UserController.IsLogin && UserController.User.user_profile.betting != null && UserController.ReferenceData != null)
		{
			var lng = UserController.ReferenceData.languages.Find(x => x.slug == code);
			var prof = UserController.User.user_profile.PostProfile;
			prof.language_id = lng.id;
			it.Api.UserApi.PostUserProfile(prof, (result) => { });
		}

		ConfirmLocale(setL.EnglishName, setL.Code);
	}

	private void ConfirmLocale(string locale, string code)
	{
		if (I2.Loc.LocalizationManager.CurrentRegionCode == code) return;
		I2.Loc.LocalizationManager.SetLanguageAndCode(locale, code);
		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.LocalizationChange);
		PlayerPrefs.SetString("LANGUAGE", code);

		//var pp = UserController.User.UserProfile.PostProfile;
		//pp.
	}

}