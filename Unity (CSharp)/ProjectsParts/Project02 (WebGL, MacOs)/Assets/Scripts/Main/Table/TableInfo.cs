using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Main;
using it.Network.Rest;
using Garilla.Games;
using it.Settings;
using System.Linq;

namespace it.UI
{
	public class TableInfo : MonoBehaviour
	{
		[SerializeField] private TableStyle _style;
		[SerializeField] private RawImage _tableImage;
		[SerializeField] private LobbyType _lobbyType;
		[SerializeField] private List<GameType> _gameType = new List<GameType>();

		public LobbyType LobbyType { get => _lobbyType; set => _lobbyType = value; }
		public List<GameType> GameType { get => _gameType; set => _gameType = value; }
		//public List<PlayerGameIcone> PlayerIcons { get => _playerIcons; set => _playerIcons = value; }

		private Table _table;
		private string _chanel;
		//private Image _back;
		private TableInfoPlaces _places;

		private void OnEnable()
		{
			//_playerIcons.ForEach(x =>
			//{
			//	x.DataRect.gameObject.SetActive(false);
			//	x.EmptyRect.gameObject.SetActive(true);
			//});

			//com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
			//ConfirmBack();
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableReserve(_chanel), OnTableReserve);
			//com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
		}

		private void UserProfileUpdate(com.ootii.Messages.IMessage handle)
		{
			ConfirmBack();
		}

		void ConfirmBack()
		{
			//if (_back == null)
			//{
			//	_back = GetComponent<Image>();
			//	if (_back == null)
			//		_back = gameObject.AddComponent<Image>();
			//}
			//_back.sprite = it.Settings.GameSettings.GameTheme.GetBackTableFast("", true);

			var ts = GameSettings.TableStyle.Find(x => x.Style == _style
			&& ((_table.is_dealer_choice && _table.is_dealer_choice == x.IsDealerChoice) || (!_table.is_dealer_choice && x.GameType.Contains((GameType)_table.game_rule_id))));

			_tableImage.texture = it.Settings.GameSettings.GameTheme.GetTableTextureFast(_style.ToString(), ts.Key,true);
		}

		public void SetData(Table record)
		{
			_table = record;
			string newChannel = SocketClient.GetChanelTable(record.id);

			if (!string.IsNullOrEmpty(_chanel) && newChannel != _chanel)
			{
				com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableReserve(_chanel), OnTableReserve);
			}
			_chanel = newChannel;
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.GameTableReserve(_chanel), OnTableReserve);

			ConfirmBack();

			if(_places != null && _places.PlayerIcons.Count != _table.players_count){
				Destroy(_places.gameObject);
				_places = null;
			}

			if(_places == null){
				var it = StandaloneSettings.TableInfoPlaces.Find(x => x.PlayerIcons.Count == _table.players_count && x.IsBig == (_style != TableStyle.RoundLobby));
				GameObject inst = Instantiate(it.gameObject, transform);
				_places = inst.GetComponent<TableInfoPlaces>();
				RectTransform rt = inst.GetComponent<RectTransform>();
				rt.localPosition = Vector3.zero;
				rt.anchoredPosition = Vector3.zero;
				rt.sizeDelta = Vector3.zero;
				rt.localScale = Vector3.one;
			}
			_places.SetData(_table);

		}


		private void OnTableReserve(com.ootii.Messages.IMessage handle)
		{
			var tableEvent = ((it.Network.Socket.GameUpdate)handle.Data).TableEvent;
			if(_places != null)
				_places.ReservedPlaces(tableEvent.table.table_player_reservations);
		}

	}
}