using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using it.Network.Rest;
using Garilla.Main;

namespace it.Popups
{

	public class SmartHudOpponentPopup : SmartHudPopup, TargetCaruselNavigationMenu
	{
		//[SerializeField] private Garilla.Main.NavigationsCarusel _carusel;
		[SerializeField] private ProfileNotes _notes;
		[SerializeField] private RectTransform _chatChange;
		[SerializeField] private NavigationsCarusel _navigationCarusel;
		[SerializeField] private DropSmilesCarusel _smiles;

		private List<Settings.GameSettings.GaneType> _settings = new List<Settings.GameSettings.GaneType>();
		private Table _table;

		private Transform _chatOnTr;
		private Transform _chatOffTr;
		private bool _isChatLock;
		private bool _waitLoadChangeBlock;

		protected override string _titleName => "popup.oponentSmartHud.title";

		protected override void Awake()
		{
			base.Awake();
			_smiles = GetComponentInChildren<DropSmilesCarusel>();
			if (_chatChange != null)
			{
				_chatOnTr = _chatChange.Find("On");
				_chatOffTr = _chatChange.Find("Off");
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			com.ootii.Messages.MessageDispatcher.AddListener(StringConstants.CHAT_BLOCKLOAD, ChatBlockLoad);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			com.ootii.Messages.MessageDispatcher.RemoveListener(StringConstants.CHAT_BLOCKLOAD, ChatBlockLoad);
		}


		private void ChatBlockLoad(com.ootii.Messages.IMessage handler)
		{
			CheckChatActive();
		}

		public void SetData(it.UI.GamePanel panel, UserLimited user, Table table, UserNote note)
		{
			_user = user;
			var gt = (GameType)table.game_rule_id;

			if (table.is_dealer_choice)
				gt = GameType.DealerChoice;

			if (_smiles != null)
			{
				_smiles.GamePanel = panel;
				_smiles.UserId = _user.id;
			}

			CheckChatActive();
			var block = it.Settings.GameSettings.Blocks.Find(y => y.TypeGame.Contains(gt));
			var rec = it.Settings.GameSettings.Games.Find(x => x.Name.ToLower() == block.Name.ToLower());
			SetDropdowm(rec.SlugRequest);
			Request(rec.SlugRequest);
			ConfirmNote(note);
		}
		public void SetData(it.UI.GamePanel panel, Table table, UserLimited user, UserStat stat, UserNote note)
		{
			_user = user;
			_table = table;

			if (_smiles != null)
			{
				_smiles.GamePanel = panel;
				_smiles.UserId = _user.id;
			}
			CheckChatActive();
			base.SetData(user, stat);
			SetDropdowm(stat.section != null ? stat.section : table.game_rule.system_id);
			ConfirmNote(note);
		}

		private void CheckChatActive()
		{
			if (_chatChange != null)
			{
				_isChatLock = UserController.Instance.ChatManager.IsUserBlock(_user.id);
				_chatOnTr.gameObject.SetActive(!_isChatLock);
				_chatOffTr.gameObject.SetActive(_isChatLock);
			}
		}

		public void SwitchChatActive()
		{
			if (_waitLoadChangeBlock) return;

			_waitLoadChangeBlock = true;
			UserController.Instance.ChatManager.ChatBlockListBlockChange(_user.id, !_isChatLock, () =>
			{
				_waitLoadChangeBlock = false;
			});
		}

		private void SetDropdowm(string slug)
		{
			if (_navigationCarusel != null)
			{
				_navigationCarusel.SelectElement(slug);
				return;
			}

			List<TMP_Dropdown.OptionData> optionsGames = new List<TMP_Dropdown.OptionData>();

			for (int i = 0; i < it.Settings.GameSettings.Games.Count; i++)
				if (it.Settings.GameSettings.Games[i].IsSmartHud)
				{
					_settings.Add(it.Settings.GameSettings.Games[i]);
					optionsGames.Add(new TMP_Dropdown.OptionData(name = it.Settings.GameSettings.Games[i].Name));
				}

			//_gameDropdown.ClearOptions();
			//if (_gameDropdown != null)
			//{
			//	_gameDropdown.onValueChanged.RemoveAllListeners();
			//	_gameDropdown.AddOptions(optionsGames);

			//	_gameDropdown.value = _settings.FindIndex(x => (!_table.is_dealer_choice && x.SlugRequest == slug)
			//	|| (_table.is_dealer_choice && x.SlugRequest == "dealer_choice"));

			//	_gameDropdown.onValueChanged.AddListener((val) =>
			//	{
			//		Request(_settings[val].SlugRequest);
			//	});
			//}
		}

		private void ConfirmNote(UserNote note)
		{
			if (_notes != null)
				_notes.Set(note);
		}

		public void SelectFromCaruselMenu(string type)
		{
			SetDropdowm(type);
			Request(type);
		}
	}
}