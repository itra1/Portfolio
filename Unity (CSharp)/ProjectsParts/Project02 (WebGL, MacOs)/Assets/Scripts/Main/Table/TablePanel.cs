using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using it.Network.Rest;
using Sett = it.Settings;
using it.Popups;
 

namespace it.UI
{
	public class TablePanel : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _buyInRange;
		[SerializeField] private RectTransform _parentTable;
		[SerializeField] private TextMeshProUGUI _infoButton;
		[SerializeField] private TableInfoPanel _infoPanel;
		[SerializeField] private TableInfo _tableInfo;
		[SerializeField] private RectTransform _lockedPanel;

		//private List<TableInfo> _tablesList = new List<TableInfo>();

		private LobbyType _selectLobby;
		private Table _selectRecord;

		//private void Awake()
		//{
		//	_tablesList = GetComponentsInChildren<TableInfo>(true).ToList();
		//}

		private void OnEnable()
		{
			_lockedPanel.gameObject.SetActive(true);
			if (_infoPanel != null)
				_infoPanel.gameObject.SetActive(false);
			_infoButton.text = "";
		}

		public void SetTableRecord(LobbyType lobby, Table record)
		{
			_infoButton.text = "";
			_tableInfo.gameObject.SetActive(record != null);
			if (record == null)
			{
				_lockedPanel.gameObject.SetActive(true);
				return;
			}
			_lockedPanel.gameObject.SetActive(false);

			_selectRecord = record;
			_selectLobby = lobby;
			_infoButton.text = "";

			ConfirmRecordData();
		}

		private void ConfirmRecordData()
		{
			var cColor = Sett.GameSettings.Colors.Find(x => x.TypeGame.Contains((GameType)_selectRecord.game_rule_id));

			//if (ti == null) return;
			var itm = Sett.GameSettings.Blocks.Find(x => x.Lobby == _selectLobby);
			if (_selectRecord.is_dealer_choice)
				_title.text = "DEALER\nCHOICE";
			else
				_title.text = cColor.NameUp;

			//ti.gameObject.SetActive(true);
			_tableInfo.SetData(_selectRecord);
			_infoButton.text = string.Format(I2.Loc.LocalizationManager.GetTranslation("lobby.preview.information"), itm.Name);

			if(_selectRecord.is_all_or_nothing)
			{
				_buyInRange.text = "lobby.preview.buyInValue".Localized() + "\n" + it.Helpers.Currency.String(_selectRecord.BuyInMaxEURO, true);
			}
			else
			_buyInRange.text = "lobby.preview.buyInRange".Localized() + "\n" + it.Helpers.Currency.String(_selectRecord.BuyInMinEURO, true) + " - " + it.Helpers.Currency.String(_selectRecord.BuyInMaxEURO, true);
		}

		public void OpenTableButton()
		{
			//Application.Quit();
			OpenGameTable();
		}

		private void OpenGameTable(bool isJoin = false)
		{

			if (_selectRecord == null) return;

			TableManager.Instance.OpenTable(_selectRecord.id, isJoin);

			//if (!UserController.IsLogin)
			//{
			//	it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
			//	return;
			//}

			//if (_selectRecord.IsVip)
			//{
			//	TablePinPopup panel = it.Main.PopupController.Instance.ShowPopup<TablePinPopup>(PopupType.TablePin);
			//	panel.Set(_selectRecord);
			//	panel.OnConfirm = (pass) =>
			//	{
			//		LobbyManager.Instance.OpenTable(_selectRecord, pass, isJoin);
			//	};
			//}
			//else
			//	TableApi.ObserveTable(_selectRecord.Id, "", (result) =>
			//	{
			//		LobbyManager.Instance.OpenTable(_selectRecord, "", isJoin);
			//	});

		}

		public void JoinTableButton()
		{
			OpenGameTable(true);
		}

		public void InformationButton()
		{
			if (_infoPanel != null)
				_infoPanel.VisiblePanel(_selectRecord);
		}

	}
}