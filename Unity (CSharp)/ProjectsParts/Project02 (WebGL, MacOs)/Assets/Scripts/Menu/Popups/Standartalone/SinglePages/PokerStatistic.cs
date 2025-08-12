using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Api;
using System.Linq;
 
using it.UI.Elements;
using com.ootii.Geometry;
using it.Inputs;
using it.Helpers;
using Garilla.Main;

namespace it.Main.SinglePages
{

	public class PokerStatistic : SinglePage, TargetCaruselNavigationMenu
	{
		[SerializeField] private StaticBlock[] _dates;
		[SerializeField] private Navigations[] _buttons;
		private int _index = -1;

		[System.Serializable]
		public struct Navigations
		{
			public string Key;
			public bool IsDevelop;
			public GraphicButtonUI Button;
		}

		[System.Serializable]
		public struct StaticBlock
		{
			public StatisticItem Item;
			public Transform Transforn;
			public string Date;
		}

		private void Awake()
		{
			for (int i = 0; i < _buttons.Length; i++)
			{
				int index = i;
				var rec = _buttons[index];
				if (rec.Button != null)
				{
					rec.Button.OnClick.RemoveAllListeners();
					rec.Button.OnClick.AddListener(() =>
					{
						SelectGame(index);
						GetData(rec.Key);
					});
				}
			}
			CreateConponents();
		}

		[ContextMenu("CreateConponents")]
		public void CreateConponents()
		{
			for (int i = 0; i < _dates.Length; i++)
			{
				_dates[i].Item = _dates[i].Transforn.GetOrAddComponent<StatisticItem>();
				_dates[i].Item.FindConponents();
			}
		}

		protected override void EnableInit()
		{
			base.EnableInit();
			for (int i = 0; i < _dates.Length; i++)
			{
				_dates[i].Item.SetData(null);
			}
			SelectGame(-1);
		}

		public void SelectGame(int index)
		{
			_index = index;
			Color tr = Color.white;
			tr.a = 0.5f;
			for (int i = 0; i < _buttons.Length; i++)
			{
			if(_buttons[i].Button != null)
				_buttons[i].Button.StartColor = _index == i || _index == -1 ? Color.white : tr;
			}

		}

		private void GetData(string game)
		{
			for (int i = 0; i < _dates.Length; i++)
			{
				_dates[i].Item.SetData(null);
			}
			UserApi.GetPokerStatistic(game, (result) =>
			{
				if (!result.IsSuccess) return;

				for (int i = 0; i < _dates.Length; i++)
				{
					switch (_dates[i].Date)
					{
						case "day":
							_dates[i].Item.SetData(result.Result.day);
							break;
						case "week":
							_dates[i].Item.SetData(result.Result.week);
							break;
						case "month":
							_dates[i].Item.SetData(result.Result.month);
							break;
						case "year":
						default:
							_dates[i].Item.SetData(result.Result.year);
							break;
					}
				}
			});
		}

		public void SelectFromCaruselMenu(string type)
		{
			GetData(type);
		}

		public class StatisticItem : MonoBehaviour
		{
			[SerializeField] private TextMeshProUGUI _averageValue;
			[SerializeField] private TextMeshProUGUI _valueLabel;
			[SerializeField] private TextMeshProUGUI _handsValue;
			[SerializeField] private CurrencyLabel _rakeValue;
			[SerializeField] private Transform _contentRect;
			[SerializeField] private Transform _noDataRect;
			[SerializeField] private Transform _diagramUp;
			[SerializeField] private Transform _diagramDown;

			public void FindConponents()
			{
				//_averageValue = transform.Find("Header/AverageLimit").GetComponent<TextMeshProUGUI>();
				_valueLabel = transform.Find("Content/Value").GetComponent<TextMeshProUGUI>();
				_handsValue = transform.Find("Content/HandsValue").GetComponent<TextMeshProUGUI>();
				_rakeValue = transform.Find("Content/RakeValue").GetComponent<CurrencyLabel>();
				_contentRect = transform.Find("Content");
				_noDataRect = transform.Find("NoData");
				_diagramUp = transform.Find("Content/DiagramUp");
				_diagramDown = transform.Find("Content/DiagramDown");
			}

			public void SetData(it.Network.Rest.PokerStatistic.DateValues data = null)
			{
				if (_contentRect == null)
					FindConponents();

				if (data == null)
				{
					_contentRect.gameObject.SetActive(false);
					_noDataRect.gameObject.SetActive(true);
					return;
				}

				_diagramUp.gameObject.SetActive(data.value > 0);
				_diagramDown.gameObject.SetActive(data.value < 0);
				if (data.value >= 0)
				{
					_valueLabel.text = "<color=#57A53C>+</color> " + data.value.CurrencyString(true);
				}
				else if(data.value == 0)
				{
					_valueLabel.text = data.value.CurrencyString(true);
				}
				else
				{
					_valueLabel.text = "<color=#F52525>+</color> " + data.value.CurrencyString(true);
				}

				_contentRect.gameObject.SetActive(true);
				_noDataRect.gameObject.SetActive(false);
				_handsValue.text = data.hands.ToString();
				_rakeValue.SetValue("{0}", data.rake);
			}

		}
	}
}