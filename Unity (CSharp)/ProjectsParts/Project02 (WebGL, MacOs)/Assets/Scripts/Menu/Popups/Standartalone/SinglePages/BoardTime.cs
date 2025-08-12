using System;
using TMPro;
using UnityEngine;

public class BoardTime : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI firstDay;
	[SerializeField] private TextMeshProUGUI firstHour;
	[SerializeField] private TextMeshProUGUI secondHour;
	[SerializeField] private TextMeshProUGUI firstMinute;
	[SerializeField] private TextMeshProUGUI secondMinute;
	[SerializeField] private TextMeshProUGUI firstSecond;
	[SerializeField] private TextMeshProUGUI secondSecond;

	private float _timer;

	void Update() => UpdateTimerDisplay();


	private void UpdateTimerDisplay()
	{
		string answer = string.Format("{00:00}{01:00}{02:00}{03:00}",
				CountTimeToNextMonday().Days,
				CountTimeToNextMonday().Hours,
				CountTimeToNextMonday().Minutes,
				CountTimeToNextMonday().Seconds);

		firstDay.text = answer[1].ToString();
		firstHour.text = answer[2].ToString();
		secondHour.text = answer[3].ToString();
		firstMinute.text = answer[4].ToString();
		secondMinute.text = answer[5].ToString();
		firstSecond.text = answer[6].ToString();
		secondSecond.text = answer[7].ToString();
	}

	private TimeSpan CountTimeToNextMonday()
	{
		var nextMonday = GetNextWeekday(DateTime.Today, DayOfWeek.Monday);

		DateTime start = DateTime.Now;
		var timeSpan = nextMonday.Subtract(start);
		return timeSpan;
	}

	private static DateTime GetNextWeekday(DateTime today, DayOfWeek dayOfWeek)
	{

		int daysUnitilNextMonday = ((int)dayOfWeek) - ((int)today.DayOfWeek - +7) % 7;
		if (daysUnitilNextMonday == 0)
		{
			return today.AddDays(daysUnitilNextMonday + 7);
		}

		return today.AddDays(daysUnitilNextMonday);
	}
}