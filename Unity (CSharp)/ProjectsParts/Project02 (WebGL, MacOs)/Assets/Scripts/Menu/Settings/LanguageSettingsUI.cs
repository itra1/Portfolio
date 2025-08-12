using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSettingsUI : SettingsPage
{
	[SerializeField] private Toggle[] toggles;
	private List<Language> languages;
	private Language languageNow;

	public void Show()
	{
		languages = UserController.ReferenceData.languages;

		languageNow = GameHelper.UserProfile.language;
		languageNow = new Language() { id = languageNow.id, flag = languageNow.flag, name = languageNow.name, slug = languageNow.slug };
		for (int i = 0; (i < toggles.Length && i < languages.Count); i++)
		{
			if (languageNow.id == languages[i].id)
			{
				toggles[i].isOn = true;
				break;
			}
		}
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void SelectLanguage(Toggle toggle)
	{
		if (toggle.isOn == false) return;

		for (int i = 0; i < toggles.Length; i++)
		{
			if (toggles[i] == toggle)
			{
				languageNow = languages[i];
				break;
			}
		}
	}

	public override void Apply()
	{
		userProfilePost.language_id = languageNow.id;
		base.Apply();
	}
	public override void Cancel()
	{
		base.Cancel();
	}
}
