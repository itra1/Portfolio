using System;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Globalization;
using Sett = it.Settings;

public class Clocks : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _timeLable;

	private float timeToRefresh = 1f;
	private CultureInfo culture = new CultureInfo("en-us");
	void Start()
	{
		StartCoroutine(TimeCoroutine());
		OnLocalizeEvent();
	}

	public void OnEnable()
	{
		I2.Loc.LocalizationManager.OnLocalizeEvent += OnLocalizeEvent;
	}

	public void OnDisable()
	{
		I2.Loc.LocalizationManager.OnLocalizeEvent -= OnLocalizeEvent;
	}

	private void OnLocalizeEvent()
	{

		var lng = Sett.AppSettings.Languages.Find(x => x.Code == I2.Loc.LocalizationManager.CurrentLanguageCode);

		culture = new CultureInfo(lng.IsoCode);
		SetTime();
	}

	IEnumerator TimeCoroutine()
	{
		while (true)
		{
			SetTime();
			yield return new WaitForSeconds(1);
		}
	}

	private void SetTime()
	{
		_timeLable.text = DateTime.Now.ToString("MMM d, <color=#C68C43>HH:mm", culture);
	}

	//private void UpdateTextWithoutColon()
	//{
	//	//text.text = DateTime.Now.ToString("MMM d, HH:mm");
	//	text.text = DateTime.Now.ToString("MMM d, <color=#C68C43>HH:mm");
	//	Invoke(nameof(UpdateTextWithoutColon), timeToRefresh);
	//}
}
