using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using it.Network.Rest;

namespace it.UI.Elements
{
	public class Bets : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _valueLabel;
		[SerializeField] private RectTransform _backLabelRect;
		[SerializeField] private CanvasGroup _cg;
		[SerializeField] private Image _sprite;
		[SerializeField] private RectTransform _content;
		public bool WaitChange;
		public RectTransform Content { get => _content; set => _content = value; }
		public decimal Value => _value + _tempValue;

		public bool VisibleValue
		{
			set
			{

				_valueLabel.gameObject.SetActive(value);
				_backLabelRect.GetComponent<Image>().enabled = value;
			}
		}

		public CanvasGroup Cg { get => _cg; set => _cg = value; }

		private decimal _value;
		private decimal _tempValue;

		private Table _table;

		public void SetValue(decimal value)
		{
			SetValue(_table, value);
		}
		public void AddValue(Table table, decimal value)
		{
			SetValue(table, _value + value);
		}
		public void SetTable(Table table)
		{
			_table = table;
		}

		public void SetValue(Table table, decimal value)
		{
			_value = value;

			if (table != null)
				_table = table;

			if (_table == null) return;

			SetValue($"{it.Helpers.Currency.String(_value)}", _table, value);
			ConfirmChip(_table, value);
		}

		public void SetValue(string valueString, Table table, decimal value)
		{
			if (table != null)
				_table = table;



			if (_table != null)
				ConfirmChip(_table, value);
			_valueLabel.text = valueString;
			_backLabelRect.sizeDelta = new Vector2(_valueLabel.preferredWidth + 20, _backLabelRect.sizeDelta.y);
		}

		void ConfirmChip(Table table, decimal value)
		{

			string sprKey = "AoN";

			if (table.is_all_or_nothing)
				sprKey = "AoN";
			else
			{
				try
				{
					var elem = it.Settings.GameSettings.GameNames.Find(x => x.GameType == (GameType)table.game_rule_id);

					if (elem == null)
						throw new System.Exception($"Не найдено имя по типу {(GameType)table.game_rule_id}");

					if (!UserController.ReferenceData.create_vip_table_options.ContainsKey(elem.Slug))
						elem = it.Settings.GameSettings.GameNames.Find(x => x.GameType == GameType.Holdem);

					if (elem != null && UserController.ReferenceData.create_vip_table_options.ContainsKey(elem.Slug))
					{
						var opt = UserController.ReferenceData.create_vip_table_options[elem.Slug];

						if (opt == null)
							throw new System.Exception($"Не найдены настройки вит таблицы по slug {elem.Slug} общие данные {Newtonsoft.Json.JsonConvert.SerializeObject(UserController.ReferenceData.create_vip_table_options)}");

						foreach (var key in opt.levels.Keys)
						{
							//if (opt.levels[key].min_buy_in == table.BuyInMin && opt.levels[key].max_buy_in == table.BuyInMax)
							//	sprKey = opt.levels[key].name;
							if (opt.levels[key].small_blind <= table.SmallBlindSize /*&& opt.levels[key].max_buy_in == table.BuyInMax*/)
								sprKey = opt.levels[key].name;
						}
					}
				}
				catch (System.Exception ex)
				{
					it.Logger.LogError("Error confirm chip " + ex.Message + " : " + ex.StackTrace);
				}
			}
			Sprite spr = it.Settings.GameSettings.Instance.GetChipSprite(sprKey, (double)value);
			if (_sprite != null)
			{
				_sprite.sprite = spr;
				RectTransform rt = _sprite.GetComponent<RectTransform>();
				rt.sizeDelta = new Vector2(spr.rect.width, spr.rect.height);
			}
		}

		public float Width()
		{
			return _backLabelRect.rect.width;
		}

		#region Animations

		public void SetVisibleAnimate(float timeAnimate = 0.5f)
		{
			_cg.alpha = 0;
			DOTween.To(() => _cg.alpha, (x) => _cg.alpha = x, 1, timeAnimate);
		}

		#endregion
	}
}