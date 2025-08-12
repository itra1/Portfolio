using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace it.Main.SinglePages
{
	public class Rank : SinglePage
	{
		//public override UserProfilePageType PageType => UserProfilePageType.Rank;

		[SerializeField] private RectTransform _mainPage;
		[SerializeField] private RectTransform _listPage;
		[SerializeField] private ScrollRect _scroll;
		[SerializeField] private RankItem _prefab;
		[SerializeField] private SwitcherPages[] _pages;
		[SerializeField] private Color _selectPageLabel;
		[SerializeField] private RankCarusel _carusel;

		private PoolList<RankItem> _pooler;

		private List<RankItem> _spawnList = new List<RankItem>();

		[System.Serializable]
		private struct SwitcherPages
		{
			public it.UI.Elements.FilterSwitchButtonUI Button;
			public RectTransform Page;
		}

		public void SelectPage(int val)
		{

			for (int i = 0; i < _pages.Length; i++)
			{
				_pages[i].Button.IsSelect = i == val;
				_pages[i].Page.gameObject.SetActive(i == val);
			}

		}

		protected override void EnableInit()
		{
			base.EnableInit();

			for (int i = 0; i < _pages.Length; i++)
			{
				int index = i;
				_pages[index].Button.OnClickPointer.RemoveAllListeners();
				_pages[index].Button.OnClickPointer.AddListener(() =>
				{
					if (index == 0)
						_carusel.CustomMuve = true;
					if (index == 1)
						_carusel.CustomMuve = false;
					SelectPage(index);
				});

			}

			SelectPage(1);
			PrintHistory();
			SelectPage(0);
			FillCard(UserController.Instance.Rank.current_rank.id);
		}

		private void FillCard(ulong id)
		{
			var c = UserController.ReferenceData.ranks.Find(x => x.id == id);
			//_card.sprite = c.Card;
		}

		private void PrintHistory()
		{

			if (_pooler == null)
				_pooler = new PoolList<RankItem>(_prefab, _scroll.content);

			_pooler.HideAll();
			_spawnList.Clear();

			var rankList = UserController.User.user_profile.rank_records;

			_prefab.gameObject.SetActive(false);
			int index = -1;
			foreach (var rec in rankList)
			{
				index++;
				var itm = _pooler.GetItem();
				itm.SetData(rec, index == 0);
				itm.OnClickAction = (rec) =>
				{
					ConfirmSelect(rec);

#if !UNITY_STANDALONE
					SelectPage(0);
#endif

					it.Logger.Log("Id" + rec.rank.id);
					_carusel.SelectCard((int)rec.rank.id);
				};
				itm.gameObject.SetActive(true);
				_spawnList.Add(itm);
			};
#if UNITY_STANDALONE
			ConfirmSelect(rankList[0]);
#endif

			var hight = _prefab.GetComponent<RectTransform>().rect.height;
			_scroll.content.sizeDelta = new Vector2(_scroll.content.sizeDelta.x, hight * (_spawnList.Count / 2));

		}

		private void ConfirmSelect(RankRecord rec)
		{
			foreach (var elem in _spawnList)
				elem.SetSelect(elem.Record.id == rec.id);
			FillCard((ulong)rec.rank_id);
		}


	}
}