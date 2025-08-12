using System.Collections;
using UnityEngine;
using TMPro;
 

namespace it.Main.SinglePages
{
	public class RankItem : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction<RankRecord> OnClickAction;

		[SerializeField] private RectTransform _selectBack;
		[SerializeField] private TextMeshProUGUI _titleLabel;
		[SerializeField] private TextMeshProUGUI _spsLabel;
		[SerializeField] private TextMeshProUGUI _dateLabel;

		private RankRecord _record;
		private bool _isFirst;

		public RankRecord Record { get => _record; set => _record = value; }

		private void OnEnable()
		{
#if UNITY_STANDALONE
		if(_isFirst)
			OnClickAction?.Invoke(_record);
#endif
		}

		public void SetData(RankRecord record, bool isFirst = false)
		{
			_isFirst = isFirst;
			_record = record;
			if (_selectBack != null)
				_selectBack.gameObject.SetActive(false);
			var cur = it.Settings.UserSettings.Ranks.Find(x => x.Id == record.rank_id);

			var recRank = UserController.ReferenceData.ranks.Find(x => x.id == record.rank_id);
			if (isFirst)
			{
				//var recRankNext = UserController.Instance.ReferenceData.Ranks.Find(x => x.Id + 1 == record.RankId);
				//if (recRankNext != null)
				//	_spsLabel.text = $"{it.Helpers.Currency.String(recRank.Price)}/{it.Helpers.Currency.String(recRankNext.Price)} SPs";
				//else
				_spsLabel.text = $"{it.Helpers.Currency.String(recRank.price, false)} SPs";
			}
			else
			{
				_spsLabel.text = $"{it.Helpers.Currency.String(recRank.price, false)} SPs";
			}

			_dateLabel.text = record.CreateDate.ToString("dd.MM.yy");
			_titleLabel.text = cur.LocalTitle.Localized();
		}

		public void ClickTouch()
		{
			OnClickAction?.Invoke(_record);
		}

		public void SetSelect(bool isSelect)
		{
			if (_selectBack != null)
				_selectBack.gameObject.SetActive(isSelect);
		}


	}
}