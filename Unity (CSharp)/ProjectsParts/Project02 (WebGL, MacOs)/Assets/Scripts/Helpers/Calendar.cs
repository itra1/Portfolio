using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Отрисовка календаря
/// </summary>
//[ExecuteInEditMode]
public class Calendar : MonoBehaviour
{
	public UnityEngine.Events.UnityAction<System.DateTime> OnSelectDay;
	public UnityEngine.Events.UnityAction<System.DateTime, System.DateTime> OnSelectPeriod;

	[SerializeField] public SelectType _selectType;
	[SerializeField] private TextMeshProUGUI _monthLabel;
	[SerializeField] private RectTransform _daysContent;
	[SerializeField] private it.UI.Elements.GraphicButtonUI _dayItem;

	private PoolList<it.UI.Elements.GraphicButtonUI> _poolerDays;
	private System.DateTime _currentMonth;
	private System.DateTime _startDate;
	private System.DateTime _endDate;

	private DateItem _startSelect;
	private DateItem _endSelect;
	private DateItem[] _dayArray;

	private bool _isStart = false;
	private class DateItem
	{
		public RectTransform Rect;
		public System.DateTime Date;
	}

	public enum SelectType
	{
		day, period
	}

	private void OnEnable()
	{
		_currentMonth = System.DateTime.Now;
		_dayItem.gameObject.SetActive(false);
		_isStart = false;

		if (_poolerDays == null)
			_poolerDays = new PoolList<it.UI.Elements.GraphicButtonUI>(_dayItem, _daysContent);

		ShowMonday();
		DrawGridDays();
	}


	public void NextMonthTouch()
	{
		_currentMonth = _currentMonth.AddMonths(1);
		ShowMonday();
		DrawGridDays();
	}

	public void PrevMonthTouch()
	{
		_currentMonth = _currentMonth.AddMonths(-1);
		ShowMonday();
		DrawGridDays();
	}

	public void ApplyTouch()
	{
		if (_selectType == SelectType.day)
			OnSelectDay?.Invoke(_startDate);

		if (_selectType == SelectType.period)
			OnSelectPeriod?.Invoke(_startDate, _endDate);
	}

	private void ShowMonday()
	{
		_monthLabel.text = _currentMonth.Date.ToString("MMM, yyyy").ToUpper();
	}

	private void DrawGridDays()
	{

		_poolerDays.HideAll();
		_dayArray = new DateItem[0];

		System.DateTime nextMonth = _currentMonth.AddMonths(1);
		int startDayFoMonth = (int)_currentMonth.AddDays(-_currentMonth.Day).DayOfWeek;
		int endtDayFoMonth = (int)nextMonth.AddDays(-_currentMonth.Day).DayOfWeek;
		System.DateTime startDate = _currentMonth.AddDays(-_currentMonth.Day).AddDays(-startDayFoMonth + 1);
		System.DateTime endDate = nextMonth.AddDays(-_currentMonth.Day).AddDays(7 - endtDayFoMonth);

		int days = (int)(endDate - startDate).TotalDays;
		_dayArray = new DateItem[days+1];

		for (int i = 0; i <= days; i++)
		{
			int index = i;
			var item = _poolerDays.GetItem();
			var txt = item.GetComponentInChildren<TextMeshProUGUI>();
			txt.text = startDate.Day.ToString();
			txt.color = startDate.Month == _currentMonth.Month ? Color.white : Color.gray;

			_dayArray[i] = new DateItem(){ Rect = item.GetComponent<RectTransform>() , Date = startDate};

			var curDate = startDate;

			item.OnClick.RemoveAllListeners();
			item.OnClick.AddListener(() =>
			{
				it.Logger.Log(curDate);
				SelectDate(_dayArray[index]);
			});

			startDate = startDate.AddDays(1);

		}

	}

	private void SelectDate(DateItem dayItem)
	{
		DateItem targetItem = null;
		if (_selectType == SelectType.day)
			targetItem = _startSelect;
		if (_selectType == SelectType.period){
			_isStart = !_isStart;
			targetItem = _isStart ? _startSelect : _endSelect;
		}


		if (targetItem != null)
		{
			targetItem.Rect.Find("Select").gameObject.SetActive(false);
			targetItem.Rect.Find("Back").gameObject.SetActive(false);
		}

		targetItem = dayItem;
		targetItem.Rect.Find("Select").gameObject.SetActive(true);
	}

}