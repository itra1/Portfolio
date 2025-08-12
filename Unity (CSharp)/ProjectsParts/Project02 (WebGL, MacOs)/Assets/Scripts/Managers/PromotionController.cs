using System.Collections;
using System.Collections.Generic;
using System.Linq;
using it.Api;
using it.Network.Rest;
using it.UI;
using UnityEngine;
using Garilla.Promotions;

public class PromotionEventData{
	public PromotionInfoCategory Category;
	public bool IsIncrement;
	public int Limit;
	public decimal Value;
	public ulong TableId;
	public ulong UserId;
	
}

public class PromotionController : AutoCreateInstance<PromotionController>
{
	public const int MICRO_3BET_AMOUNT = 25;
	public const int AVERAGE_3BET_AMOUNT = 50;
	public const int HIGH_3BET_AMOUNT = 100;
	public const int MICRO_WTSD_AMOUNT = 20;
	public const int AVERAGE_WTSD_AMOUNT = 40;
	public const int HIGH_WTSD_AMOUNT = 80;
	public const int MICRO_AoN_AMOUNT = 20;
	public const int AVERAGE_AoN_AMOUNT = 100;
	public const int HIGH_AoN_AMOUNT = 200;
	public const int MICRO_GM_AMOUNT = 50;
	public const int AVERAGE_GM_AMOUNT = 100;
	public const int HIGH_GM_AMOUNT = 300;

	private List<PromotionIcone> _promotionIcones;

	public event UnityEngine.Events.UnityAction<PromotionsData> OnUpdate;
	//public event UnityEngine.Events.UnityAction<PromotionInfoCategory, string, int, decimal, ulong, ulong> OnAward;
	//public event UnityEngine.Events.UnityAction<PromotionInfoCategory, string, int, ulong> OnIncrement;
	public event UnityEngine.Events.UnityAction<PromotionEventData> OnPromotion;

	private PromotionsData _data;

	public PromotionsData Data { get => _data; set => _data = value; }

	private void Start()
	{
		GetData();

		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserLogin, UserLogin);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, UserLogin);
		LoadPromotionsIcons();
	}

	public PromotionIcone GetPrefabiconeByType(PromotionInfoCategory type){
		return _promotionIcones.Find(x => x.Type == type);
	}

	private void LoadPromotionsIcons(){
		_promotionIcones = Garilla.ResourceManager.GetResourceAll<PromotionIcone>("Prefabs/Promotions").ToList();
		it.Logger.Log("Promotions list " + _promotionIcones.Count);
	}

	private void OnDestroy()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserLogin, UserLogin);
	}

	private void UserLogin(com.ootii.Messages.IMessage handler)
	{
		GetData();
	}

	public void GetData(UnityEngine.Events.UnityAction OnComplete = null)
	{
		if (!UserController.IsLogin) return;

		UserApi.GetPromotions((ResultResponse<PromotionsData> response) =>
		{
			_data = null;

			if (response.IsSuccess)
				_data = response.Result;

			OnComplete?.Invoke();

			OnUpdate?.Invoke(_data);
		});

	}

	/// <summary>
	/// Запуск промотионт
	/// </summary>
	public void OpenPronotionsPanel()
	{

		if (!UserController.IsLogin)
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
			return;
		}

		it.Main.SinglePageController.Instance.Show(SinglePagesType.Promotions);
	}
	[ContextMenu("Set text data")]
	public void SetTextData(){
		//SetIncrement(PromotionInfoCategory.Bet_Race, "micro", 1);
		//SetIncrement(PromotionInfoCategory.Bet_Race, "average", 2);
		//SetIncrement(PromotionInfoCategory.Bet_Race, "high", 3);
		//SetIncrement(PromotionInfoCategory.Poker_Hands, "high", 3);
		//SetAwawrd(PromotionInfoCategory.Bet_Race, "micro", 101m);
		//SetAwawrd(PromotionInfoCategory.Bet_Race, "average", 102m);
		//SetAwawrd(PromotionInfoCategory.Bet_Race, "high", 1013m);
		//SetAwawrd(PromotionInfoCategory.Game_Manager, 101);
		//SetAwawrd(PromotionInfoCategory.Poker_Hands, "micro", 101);
	}
	public float GetCounter(PromotionInfoCategory type, string typeTable)
	{
		if (_data == null)
			return 0;

		switch (type)
		{
			case PromotionInfoCategory.AoN_Race:
				return _data.aon_race.Find(x => x.level == typeTable).count;
			case PromotionInfoCategory.WT_Race:
				return _data.wtsd_race.Find(x => x.level == typeTable).count;
			case PromotionInfoCategory.Bet_Race:
				return _data.three_bet.Find(x => x.level == typeTable).count;
			case PromotionInfoCategory.Game_Manager:
				return _data.game_manager.Find(x => x.level == typeTable).count;
		}
		return 0;

	}

	public void SetAwawrd(PromotionInfoCategory type, ulong userId, ulong tableId, string typeTable, decimal award)
	{
		if (_data == null)
			return;

		switch (type)
		{
			case PromotionInfoCategory.AoN_Race:
				var itm = _data.aon_race.Find(x => x.level == typeTable);
				Debug.Log("On emit SetAwawrd OnPromotion 1");
				OnPromotion?.Invoke(new PromotionEventData() { Category = type, IsIncrement = false, Limit = (int)itm.limit, TableId = tableId, UserId = userId, Value = award });
				//OnAward?.Invoke(type, typeTable, (int)itm.limit, award);
				break;
			case PromotionInfoCategory.WT_Race:
				var itm1 = _data.wtsd_race.Find(x => x.level == typeTable);
				Debug.Log("On emit SetAwawrd OnPromotion 2");
				OnPromotion?.Invoke(new PromotionEventData() { Category = type, IsIncrement = false, Limit = (int)itm1.limit, TableId = tableId, UserId = userId, Value = award });
				//OnAward?.Invoke(type, typeTable, (int)itm1.limit, award);
				break;
			case PromotionInfoCategory.Bet_Race:
				var itm2 = _data.three_bet.Find(x => x.level == typeTable);
				Debug.Log("On emit SetAwawrd OnPromotion 3");
				OnPromotion?.Invoke(new PromotionEventData() { Category = type, IsIncrement = false, Limit = (int)itm2.limit, TableId = tableId, UserId = userId, Value = award });
				//OnAward?.Invoke(type, typeTable, (int)itm2.limit, award);
				break;
			case PromotionInfoCategory.Game_Manager:
				var itm3 = _data.game_manager.Find(x => x.level == typeTable);
				Debug.Log("On emit SetAwawrd OnPromotion 4");
				OnPromotion?.Invoke(new PromotionEventData() { Category = type, IsIncrement = false, Limit = (int)itm3.limit, TableId = tableId, UserId = userId, Value = award });
				//OnAward?.Invoke(type, typeTable, (int)itm3.limit, award);
				break;
			case PromotionInfoCategory.Poker_Hands:
				Debug.Log("On emit SetAwawrd OnPromotion 5");
				OnPromotion?.Invoke(new PromotionEventData() { Category = type, IsIncrement = false, Limit = 0, TableId = tableId, UserId = userId, Value = award });
				//OnAward?.Invoke(type, typeTable, 0, award);
				break;
		}

	}

	public void SetIncrement(PromotionInfoCategory type, string typeTable, int count, ulong tableId)
	{
		if (_data == null)
			return;

		switch (type)
		{
			case PromotionInfoCategory.AoN_Race:
				var itm = _data.aon_race.Find(x => x.level == typeTable);
				itm.count = count;
				Debug.Log("On emit SetIncrement OnPromotion 1");
				OnPromotion?.Invoke(new PromotionEventData() { Category = type, IsIncrement = true, TableId = tableId, Value = count });
				//OnIncrement?.Invoke(type, typeTable, count, tableId);
				break;
			case PromotionInfoCategory.WT_Race:
				var itm1 = _data.wtsd_race.Find(x => x.level == typeTable);
				itm1.count = count;
				Debug.Log("On emit SetIncrement OnPromotion 2");
				OnPromotion?.Invoke(new PromotionEventData() { Category = type, IsIncrement = true, TableId = tableId, Value = count });
				//OnIncrement?.Invoke(type, typeTable, count, tableId);
				break;
			case PromotionInfoCategory.Bet_Race:
				var itm2 = _data.three_bet.Find(x => x.level == typeTable);
				itm2.count = count;
				Debug.Log("On emit SetIncrement OnPromotion 3");
				OnPromotion?.Invoke(new PromotionEventData() { Category = type, IsIncrement = true, TableId = tableId, Value = count });
				//OnIncrement?.Invoke(type, typeTable, count, tableId);
				break;
			case PromotionInfoCategory.Game_Manager:
				var itm3 = _data.game_manager.Find(x => x.level == typeTable);
				itm3.count = count;
				Debug.Log("On emit SetIncrement OnPromotion 4");
				OnPromotion?.Invoke(new PromotionEventData() { Category = type, IsIncrement = true, TableId = tableId, Value = count });
				//OnIncrement?.Invoke(type, typeTable, count, tableId);
				break;
			case PromotionInfoCategory.Poker_Hands:
				Debug.Log("On emit SetIncrement OnPromotion 5");
				OnPromotion?.Invoke(new PromotionEventData() { Category = type, IsIncrement = true, TableId = tableId, Value = count });
				//OnIncrement?.Invoke(type, typeTable, count, tableId);
				break;
		}

	}

}