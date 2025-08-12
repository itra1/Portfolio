using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeBankSettingsUI : SettingsPage
{
	[SerializeField] private Toggle[] toggles;
	[SerializeField] private TextMeshProUGUI[] titles;
	[SerializeField] private TextMeshProUGUI[] descriptions;

	private List<Classifier> classifiers;
	private int id;
	private int select;

	public void Show()
	{
		classifiers = UserController.ReferenceData.time_bank_types;
		id = GameHelper.UserProfile.time_bank_type_id;

		for (int i = 0; i < classifiers.Count; i++)
		{
			titles[i].text = classifiers[i].short_name;
			descriptions[i].text = classifiers[i].name;
			if (classifiers[i].id == id)
			{
				select = i;
			}
		}
		toggles[select].isOn = true;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void SetTimeBank(int n)
	{
		select = n;
		id = classifiers[n].id;
	}

	public override void Apply()
	{
		userProfilePost.time_bank_type_id = id;
		base.Apply();
	}
}
