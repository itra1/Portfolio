using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 

namespace it.Main.SinglePages
{
	public class RankCard : MonoBehaviour
	{
		[SerializeField] private RectTransform _maxRank;
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private Image _backImage;
		[SerializeField] private TextMeshProUGUI _titleLabel;
		[SerializeField] private TextMeshProUGUI _daysLabel;
		[SerializeField] private TextMeshProUGUI _valueLabel;
		[SerializeField] private TextMeshProUGUI _spsLabel;
		[SerializeField] private Image _fillImage;

		public CanvasGroup CanvasGroup { get => _canvasGroup; set => _canvasGroup = value; }
		public Image FillImage { get => _fillImage; set => _fillImage = value; }
		public RectTransform Rt
		{
			get
			{

				if (_rt == null)
					_rt = GetComponent<RectTransform>();
				return _rt;
			}
			set => _rt = value;
		}

		private RectTransform _rt;


		public void SetData(int id, RankItemResponse rank)
		{
			if (rank == null)
			{
				if (_maxRank != null)
					_maxRank.gameObject.SetActive(true);
				return;
			}
			if (_maxRank != null)
				_maxRank.gameObject.SetActive(false);
			var cur = it.Settings.UserSettings.Ranks.Find(x => x.Id == rank.id);

			var rec = it.Settings.GameSettings.RankSettings.Find(x => x.Slug == rank.slug);

			if (_backImage != null)
				_backImage.sprite = rec.GetSprite(id);

			if (_titleLabel != null)
				_titleLabel.text = cur.LocalTitle.Localized().ToUpper();
			if (_daysLabel != null)
				_daysLabel.text = rank.slug == "knight" ? "rank.names.start".Localized() : "rank.names.7days".Localized();
			if (_valueLabel != null)
				_valueLabel.text = rank.rakeback.ToString();
			if (_spsLabel != null)
				_spsLabel.text = it.Helpers.Currency.String(rank.price, false) + " " + "rank.names.sps".Localized();

		}


	}
}