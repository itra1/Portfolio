using System;
using System.Collections.Generic;
using System.Linq;

using I2.Loc;

using it.Api;
using it.Main.SinglePages;
using it.Network.Rest;

using UnityEngine;

namespace it.UI.Promotions
{
	public class PromotionsPage : SinglePage
	{
		[SerializeField] private NavigationButton[] _buttons;

		private Dictionary<PromotionInfoCategory, NavigationButton> _pages = new Dictionary<PromotionInfoCategory, NavigationButton>();

		private PromotionsData _data;

		private void Awake()
		{
			InitPages();
			PromotionController.Instance.GetData(() =>
			{
				UpdateData(PromotionController.Instance.Data);
			});
		}

		private void Start()
		{
			PromotionController.Instance.OnUpdate += UpdateData;
		}

		private void OnDestroy()
		{
			PromotionController.Instance.OnUpdate -= UpdateData;
		}

		private void UpdateData(PromotionsData data)
		{
			_data = data;

			foreach (var pageData in _pages)
			{
				UpdatePage(pageData.Value.page);
			}
		}

		protected override void EnableInit()
		{
			base.EnableInit();
		}

		private void InitPages()
		{
			foreach (var navigationButton in _buttons)
			{
				if (navigationButton.button != null &&
						navigationButton.page != null &&
						_pages.ContainsKey(navigationButton.page.category) == false &&
						navigationButton.page.category !=
						PromotionInfoCategory.None)
				{
					navigationButton.button.OnClick.RemoveAllListeners();
					navigationButton.button.OnClick.AddListener(delegate
					{
						ShowPage(navigationButton.page.category);
					});

					_pages.Add(navigationButton.page.category, navigationButton);
				}
			}

			HideAllPages();
			if (_pages.Count > 0)
			{
				ShowPage(_pages.First().Value.page.category);

				//foreach (var pageData in _pages)
				//{
				//	UpdatePage(pageData.Value.page);
				//}
			}
		}

		public void UpdatePage(PromotionInfoBasePage page)
		{
			switch (page.category)
			{
				case PromotionInfoCategory.None:
				case PromotionInfoCategory.Poker_Hands:
					{
						break;
					}
				case PromotionInfoCategory.Bet_Race:
					{
						if (page is PromotionInfoDataPage infoDataPage)
						{
							infoDataPage.SetData(new List<PromotionInfoDataComponent.InfoData>()
												{
														new PromotionInfoDataComponent.InfoData("micro", new Vector2(_data.three_bet.Find(x=>x.level == "micro").count, _data.three_bet.Find(x=>x.level == "micro").limit), PromotionController.MICRO_3BET_AMOUNT),
														new PromotionInfoDataComponent.InfoData("average", new Vector2(_data.three_bet.Find(x=>x.level == "average").count, _data.three_bet.Find(x=>x.level == "average").limit), PromotionController.AVERAGE_3BET_AMOUNT),
														new PromotionInfoDataComponent.InfoData("high", new Vector2(_data.three_bet.Find(x=>x.level == "high").count, _data.three_bet.Find(x=>x.level == "high").limit), PromotionController.HIGH_3BET_AMOUNT)
												});
						}
						break;
					}
				case PromotionInfoCategory.WT_Race:
					{
						if (page is PromotionInfoDataPage infoDataPage)
						{
							infoDataPage.SetData(new List<PromotionInfoDataComponent.InfoData>()
												{
														new PromotionInfoDataComponent.InfoData("micro", new Vector2(_data.wtsd_race.Find(x=>x.level == "micro").count, _data.wtsd_race.Find(x=>x.level == "micro").limit), PromotionController.MICRO_WTSD_AMOUNT),
														new PromotionInfoDataComponent.InfoData("average", new Vector2(_data.wtsd_race.Find(x=>x.level == "average").count, _data.wtsd_race.Find(x=>x.level == "average").limit), PromotionController.AVERAGE_WTSD_AMOUNT),
														new PromotionInfoDataComponent.InfoData("high", new Vector2(_data.wtsd_race.Find(x=>x.level == "high").count, _data.wtsd_race.Find(x=>x.level == "high").limit), PromotionController.HIGH_WTSD_AMOUNT)
												});
						}
						break;
					}
				case PromotionInfoCategory.AoN_Race:
					{
						if (page is PromotionInfoDataPage infoDataPage)
						{
							infoDataPage.SetData(new List<PromotionInfoDataComponent.InfoData>()
												{
														new PromotionInfoDataComponent.InfoData("micro", new Vector2(_data.aon_race.Find(x=>x.level == "micro").count, _data.aon_race.Find(x=>x.level == "micro").limit), PromotionController.MICRO_AoN_AMOUNT),
														new PromotionInfoDataComponent.InfoData("average", new Vector2(_data.aon_race.Find(x=>x.level == "average").count, _data.aon_race.Find(x=>x.level == "average").limit), PromotionController.AVERAGE_AoN_AMOUNT),
														new PromotionInfoDataComponent.InfoData("high", new Vector2(_data.aon_race.Find(x=>x.level == "high").count, _data.aon_race.Find(x=>x.level == "high").limit), PromotionController.HIGH_AoN_AMOUNT)
												});
						}
						break;
					}
				case PromotionInfoCategory.Game_Manager:
					{
						if (page is PromotionInfoDataPage infoDataPage)
						{
							infoDataPage.SetData(new List<PromotionInfoDataComponent.InfoData>()
												{
														new PromotionInfoDataComponent.InfoData("micro", new Vector2(_data.game_manager.Find(x=>x.level == "micro").count, _data.game_manager.Find(x=>x.level == "micro").limit), PromotionController.MICRO_GM_AMOUNT,true),
														new PromotionInfoDataComponent.InfoData("average", new Vector2(_data.game_manager.Find(x=>x.level == "average").count, _data.game_manager.Find(x=>x.level == "average").limit), PromotionController.AVERAGE_GM_AMOUNT,true),
														new PromotionInfoDataComponent.InfoData("high", new Vector2(_data.game_manager.Find(x=>x.level == "high").count, _data.game_manager.Find(x=>x.level == "high").limit), PromotionController.HIGH_GM_AMOUNT, true)
												});
						}
						break;
					}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ShowPage(PromotionInfoCategory category)
		{
			if (_pages.ContainsKey(category))
			{
				HideAllPages();
				_pages[category].button.IsSelect = true;
				_pages[category].page.gameObject.SetActive(true);
			}
		}

		private void HideAllPages()
		{
			foreach (var pageData in _pages)
			{
				pageData.Value.button.IsSelect = false;
				pageData.Value.page.gameObject.SetActive(false);
			}
		}

		[System.Serializable]
		public struct NavigationButton
		{
			public Elements.FilterSwitchButtonUI button;
			public PromotionInfoBasePage page;
		}
	}
}