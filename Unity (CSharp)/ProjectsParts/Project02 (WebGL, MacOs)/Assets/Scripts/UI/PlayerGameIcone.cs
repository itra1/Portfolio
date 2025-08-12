using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Inputs;
using it.Network.Rest;
using System;
using DG.Tweening;
using it.Popups;
using it.UI;
using it.Api;
using com.ootii.Geometry;
using System.Threading.Tasks;

namespace Garilla.Games
{
	/// <summary>
	/// Интерфейс структуры с данными AFK
	/// </summary>
	public interface IMyAfk
	{
		public bool SkipDistributionsWellBeSet { get; }
		public bool SkipDistributions { get; }
		public float SkipDistributionsTime { get; }
		public bool CanSkipDistributions { get; }
		public float CanSkipDistributionsTime { get; }
	}

	public class PlayerGameIcone : MonoBehaviour
	{
		[HideInInspector] public TablePlayerSession TablePlayerSession;
		public RectTransform DataRect;
		public GamePlaceUI EmptyRect;
		[SerializeField] private Image _stateContainer;
		[SerializeField] private TextMeshProUGUI _stateText;
		[SerializeField] private TextMeshProUGUI _combinationLabel;
		[SerializeField] private TextMeshProUGUI _combinationLowLabel;
		[SerializeField] private TextMeshProUGUI _rakeLabel;
		[SerializeField] private RectTransform _straddlePanel;
		[SerializeField] private RectTransform _straddleLabel;
		[SerializeField] private RectTransform _parentWinCombination;
		[SerializeField] public Timer _timer;
		[SerializeField] private RectTransform _timeLineRect;
		[SerializeField] private Animator _bingoAnimator;
		[SerializeField] private TextMeshProUGUI _nickname;
		[SerializeField] private GameObject _activeLight;
		[SerializeField] private TextMeshProUGUI _sp;
		[SerializeField] private CurrencyLabel _balanceLabel;
		[SerializeField] private Image _sticker;
		[SerializeField] private RawImage _flag;
		[SerializeField] private RectTransform _timeLine;
		[SerializeField] private it.UI.Avatar _avatar;
		[SerializeField] private GameObject CardsContainer;
		//[SerializeField] private GameCardUI GameCardUIPrefab;
		[SerializeField] private RectTransform _winBlock;
		[SerializeField] private List<WinCombinationStruct> _winCombinations;
		[SerializeField] private List<StateItem> _stateList;
		[SerializeField] private CardGroup _cardsPositions;
		[SerializeField] private RectTransform _sittingOutLabel;
		[SerializeField] private RectTransform _allinLabel;
		[SerializeField] private PlayerChatSmile _playerSmilePrefab;
		[SerializeField] private PlayerGameAfk _playerAfk;
		[SerializeField] private AggresivePanel _aggresivePanel;
		[SerializeField] private RectTransform[] _dialogPoints;
		[SerializeField] private RectTransform _dialogRect;
		[SerializeField] private RectTransform _helloLabel;
		public bool WaitMovePlayerAnimation;
		private int _lastActiveEventId;
		private List<RectTransform> _cardPlaces;
		private TimerManager.RealTimer _realTimerAfk;
		private bool _isAfk;
		private Tween _delayLowCombination;

		[System.Serializable]
		public class CardGroup
		{
			public List<CardBlock> Cards;

			[System.Serializable]
			public class CardBlock
			{
				public List<RectTransform> CardPositions;
				public int CardsCount => CardPositions.Count;
			}

		}

		public it.UI.Avatar Avatar { get => _avatar; set => _avatar = value; }
		/// <summary>
		/// Id игркоа
		/// </summary>
		public ulong UserId { get => _userId; set => _userId = value; }
		public CardGroup CardsPositions { get => _cardsPositions; set => _cardsPositions = value; }
		public List<GameCardUI> Cards { get => _cards; set => _cards = value; }
		/// <summary>
		/// Собственный плеер
		/// </summary>
		public bool IsMe => UserController.User == null ? false : _userId == UserController.User.id;

		private ulong _userId;
		private List<GameCardUI> _cards = new List<GameCardUI>();
		private float _startTimeLineWidth;
		private DistributionSharedData _sharedData = null;
		private DistributionSharedDataPlayer _shredDataPlayer;
		private List<DistributionCard> _distributionsCards;
		private DistributionEvent _action;
		private List<DistributionPlayerCombination> _winCombinationPlayer = new List<DistributionPlayerCombination>();
		private bool _isPreFlop;
		private bool _isShowDown;
		private bool _WinSee;
		private UserNote _note;

		public bool WaitToClickToShowShow { get; set; } = false;
		public bool VisibleDropCards { get; set; } = false;

		//private List<DistributionPlayerCombination> _winCombinationsPlayer = new List<DistributionPlayerCombination>();
		[HideInInspector] public UserLimited user;
		private UserStat _userStat;

		private GameControllerBase _gameController;
		public GameControllerBase GameController
		{
			get
			{
				if (_gameController == null)
					_gameController = GetComponentInParent<GameControllerBase>();
				return _gameController;
			}
		}

		public DistributionSharedDataPlayer ShredDataPlayer { get => _shredDataPlayer; set => _shredDataPlayer = value; }
		public TimerManager.RealTimer RealTimerAfk { get => _realTimerAfk; set => _realTimerAfk = value; }
		public bool WinSee { get => _WinSee; set => _WinSee = value; }

		[System.Serializable]
		public struct StateItem
		{
			public string Name;
			public string Title;
			//public GameObject Image;
			public Color Color;
		}

		[System.Serializable]
		public struct WinCombinationStruct
		{
			public string Name;
			public GameObject Image;
		}

		private void Awake()
		{
			_flag.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			if (_dialogRect != null)
				_dialogRect.gameObject.SetActive(false);

			if (_activeLight != null)
				_activeLight.gameObject.SetActive(false);
			_startTimeLineWidth = _timeLine.rect.width;
			Clear();

			if (_helloLabel != null)
				_helloLabel.gameObject.SetActive(false);
			StraddlePanelEnable(false);
		}

		public void SpawnClear()
		{
			Clear();
			NoteClear();
			TablePlayerSession = null;
			_userId = 0;
			_avatar.SetDefaultAvatar();
			_flag.gameObject.SetActive(false);
			_timer.StopTimer();
			ClearCards();
		}

		public void Clear()
		{
			NoWin();
			ClearState();
			ClearCards();

			//_bingoAnimator.gameObject.SetActive(false);

			if (_delayLowCombination != null && _delayLowCombination.IsActive())
				_delayLowCombination.Kill();
		}

		public void Set(TablePlayerSession player, bool isShowDown = false)
		{
			//_isAfk = false;
			this.TablePlayerSession = player;
			_balanceLabel.gameObject.SetActive((!player.is_resting && !player.SkipDistributions) && TablePlayerSession.amount > 0);
			_balanceLabel.SetValue(TablePlayerSession.amount);

			ProcessAfk(TablePlayerSession);
			if (_userId == player.user_id) return;

			_userId = player.user_id;
			SetFlag(player.user.country.flag);
			NoteLoad();

			_nickname.text = TablePlayerSession.user.nickname;
			_avatar.SetDefaultAvatar();

			_allinLabel.gameObject.SetActive((!player.is_resting && !player.SkipDistributions) && TablePlayerSession.amount <= 0);
			_sittingOutLabel.gameObject.SetActive((player.is_resting || player.SkipDistributions) && TablePlayerSession.amount > 0);

			//_avatar.gameObject.SetActive(false);
			//_avatar.sprite = SpriteManager.instance.avatarDefault;

			if (string.IsNullOrEmpty(TablePlayerSession.user.AvatarUrl))
			{
				if (UserController.IsLogin)
					TableApi.GetUser(TablePlayerSession.user.id, (res) =>
					{
						_avatar.SetAvatar(res.Result.data.user_profile.avatarUrl);
					});
			}
			else
			{
				_avatar.SetAvatar(TablePlayerSession.user.AvatarUrl);
			}
			PlayerPrefs.DeleteKey(StringConstants.NoteUpdate(UserId));

			if (_aggresivePanel && TablePlayerSession.session_stat != null)
				_aggresivePanel.SerAggresiceValue(TablePlayerSession.session_stat.aggressive_distributions);

			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.MyAfkChange, MyAfkChange);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.MyAfkChange, MyAfkChange);

		}

		public void StraddlePanelEnable(bool isActive)
		{
			if (_straddlePanel != null)
				_straddlePanel.gameObject.SetActive(isActive);
		}

		public void SetDialog(string message)
		{
			var dialog = _dialogRect.gameObject.GetOrAddComponent<DialogMessages>();
			dialog.gameObject.SetActive(true);

			var parentGamePanel = GetComponentInParent<GamePanel>();
			RectTransform gpRt = parentGamePanel.GetComponent<RectTransform>();
			var ancor = gpRt.InverseTransformPoint(transform.position);

			float val = ((gpRt.rect.width / 2) + ancor.x) / gpRt.rect.width;

			bool isLeft = (val >= 0.16f && val < 0.5f) || (val >= 0.84f);

#if !UNITY_STANDALONE
			isLeft = val >= 0.5f;
#endif

			var dRect = dialog.GetComponent<RectTransform>();
			if (isLeft)
			{
				dRect.SetParent(_dialogPoints[0]);
			}
			else
			{
				dRect.SetParent(_dialogPoints[1]);
			}
			dialog.transform.localPosition = Vector3.zero;
			dRect.anchoredPosition = Vector3.zero;

			dialog.SetMessage(isLeft, message);
		}

		#region Bingo

		public bool ProcessBingo()
		{
			if (UserId == UserController.User.id) return false;

			if (_shredDataPlayer == null) return false;

			//#if UNITY_EDITOR

			//			var winIcone1 = GetComponent<PlayerGameWinIcones>();
			//			if (winIcone1 != null)
			//				winIcone1.BingoShow();

			//#endif

			if (_shredDataPlayer.bingo_game != null && _shredDataPlayer.bingo_game.is_win != null && (bool)_shredDataPlayer.bingo_game.is_win)
			{
				var winIcone = GetComponent<PlayerGameWinIcones>();
				if (winIcone != null)
					winIcone.BingoShow();
				return true;
			}
			return false;
		}

		//private void BingoShow()
		//{
		//	_bingoAnimator.gameObject.SetActive(true);
		//	_bingoAnimator.SetTrigger("visible");
		//	_bingoAnimator.GetComponent<CanvasGroup>().alpha = 1;
		//	DOVirtual.DelayedCall(4, () =>
		//	{
		//		BingoHide();
		//	});
		//}

		//private void BingoHide()
		//{
		//	var cg = _bingoAnimator.GetComponent<CanvasGroup>();
		//	DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 0, 0.4f).OnComplete(() =>
		//	{
		//		_bingoAnimator.gameObject.SetActive(false);
		//	});
		//}


		#endregion

		//private void OnDestroy()
		//{
		//	com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.MyAfkChange, MyAfkChange);
		//}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.MyAfkChange, MyAfkChange);
			if (_isLightPanel)
				HideLighInfo();
		}

#if UNITY_STANDALONE
		private void Update()
		{
			CheckNoteUpdate();
		}
#endif

		#region Notes

		private Coroutine NoteCheckUpdate;
		private string _lastNoteChange;
		private void NoteLoad()
		{
			if (!UserController.IsLogin) return;
			if (GameController != null && GameController.SelectTable.is_all_or_nothing) return;
			if (_note == null || (_note.user_id == this.TablePlayerSession.user_id))
			{
				NoteClear();
				UserApi.NoteGet(TablePlayerSession.user_id, (result) =>
				{

					if (!result.IsSuccess)
						return;

					_note = result.Result;
					_note.OnChange = () =>
					{
						NoteConfirm();
						PlayerPrefs.SetString(StringConstants.NoteUpdate(UserId), DateTime.Now.Millisecond.ToString());
						PlayerPrefs.Save();
					};
					NoteConfirm();
				});
			}
		}

		private void CheckNoteUpdate()
		{
			if (!PlayerPrefs.HasKey(StringConstants.NoteUpdate(UserId))) return;

			string str = PlayerPrefs.GetString(StringConstants.NoteUpdate(UserId));

			if (str != _lastNoteChange)
			{
				_lastNoteChange = str;
				NoteLoad();
			}
		}

		private void NoteConfirm()
		{
			if (_note == null) return;
			if (_sticker == null) return;

			if (_note.color <= 0)
			{
				_sticker.gameObject.SetActive(false);
			}
			else
			{
				_sticker.gameObject.SetActive(true);
				_sticker.color = it.Settings.AppSettings.Sticks.Sticks[_note.color].Color;
			}

		}

		private void NoteClear()
		{
			_note = null;
			if (_sticker == null) return;
			_sticker.gameObject.SetActive(false);
		}

		#endregion

		private void MyAfkChange(com.ootii.Messages.IMessage handler)
		{
			if (UserController.User == null || UserController.User.id != _userId) return;


			Garilla.Games.IMyAfk afk = (Garilla.Games.IMyAfk)handler.Data;
			ProcessAfk(afk);
		}

		public void SetAmount(decimal value)
		{
			if (TablePlayerSession == null) return;
			TablePlayerSession.amount = value;
			_balanceLabel.SetValue(TablePlayerSession.amount);
			_balanceLabel.gameObject.SetActive((!TablePlayerSession.is_resting && !TablePlayerSession.SkipDistributions) && TablePlayerSession.amount > 0);
			_allinLabel.gameObject.SetActive((!TablePlayerSession.is_resting && !TablePlayerSession.SkipDistributions) && TablePlayerSession.amount <= 0);
		}
		public void SetBufferAmount(decimal value)
		{
			if (TablePlayerSession == null) return;
			TablePlayerSession.amount_buffer = value;
		}
		public void Hide()
		{
			DOTween.To(() => transform.localScale, (x) => transform.localScale = x, Vector3.zero, 0.3f).OnComplete(() =>
			{
				PoolerManager.Return("PlayerGame", gameObject);
				//Destroy(gameObject);
			});
		}

		//public void SetRake(double value)
		//{
		//	//_rakeLabel.text = $"Rake = {value}";
		//	//_rakeLabel.gameObject.SetActive(true);
		//}

		public void Show(Transform parent)
		{
			transform.SetParent(parent);
			transform.localScale = Vector3.zero;
			transform.localPosition = Vector3.zero;

			Vector3 targetSize = Vector3.one;
			//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			//			if (UserId == UserController.User.Id)
			//				targetSize = Vector3.one * 1.5f;
			//			else
			//				targetSize = Vector3.one * 1.5f;
			//#endif

#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			if (UserId == UserController.User.id)
				targetSize = Vector3.one * 1.85f;
			else
				targetSize = Vector3.one * 1.5f;
#endif


			DOTween.To(() => transform.localScale, (x) => transform.localScale = x, targetSize, 0.3f).OnComplete(() =>
			{
				ShowHi();
			});
		}
		public void MoveParent(Transform parent)
		{
			DOTween.To(() => transform.localScale, (x) => transform.localScale = x, Vector3.zero, 0.3f).OnComplete(() =>
			{
				transform.SetParent(parent);
				transform.localScale = Vector3.zero;
				transform.localPosition = Vector3.zero;

				Vector3 targetSize = Vector3.one;
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
				if (UserId == UserController.User.id)
					targetSize = Vector3.one * 1.85f;
				else
					targetSize = Vector3.one * 1.5f;
#endif

				DOTween.To(() => transform.localScale, (x) => transform.localScale = x, targetSize, 0.3f);
			});
		}

		private async void ShowHi()
		{
			_helloLabel.localScale = Vector3.zero;
			_helloLabel.gameObject.SetActive(true);
			await Task.Delay(200);
			_helloLabel.DOScale(Vector3.one, 0.2f);
			_helloLabel.GetComponent<Animation>().Play();
			await Task.Delay(2000);
			_helloLabel.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
			{
				_helloLabel.gameObject.SetActive(false);
			});
		}

		public void Set(UserProfileLimited profile)
		{
			_sp.text = profile.sp.ToString();
		}

		public void SetDistributive(DistributionSharedDataPlayer dataPlayer, DistributionSharedData distrib, DistributionEvent action,
				List<DistributionCard> distributionCards, bool isPreFlop, bool isShowDown, bool WinSee = true)
		{
			_shredDataPlayer = null;
			_shredDataPlayer = dataPlayer;
			_sharedData = null;
			_sharedData = distrib;
			_action = null;
			_action = action;
			_distributionsCards = null;
			_distributionsCards = distributionCards;
			_isPreFlop = isPreFlop;
			_isShowDown = isShowDown;
			_WinSee = WinSee;

			if (_aggresivePanel && dataPlayer.session_stat != null)
				_aggresivePanel.SerAggresiceValue(dataPlayer.session_stat.aggressive_distributions);

			//if (_sharedData.WinCombinations.Count > 0)
			_winCombinationPlayer = new List<DistributionPlayerCombination>(_sharedData.WinCombinations);
			if (_shredDataPlayer.IsWin && !_sharedData.ExistsShowDown() && _WinSee)
				ShowWin();
			//if (_sharedData != null)
			//	InitCombinations(_shredDataPlayer.IsWin && _WinSee, _sharedData.WinCombinations);
		}

		public void ProcessDistributive(bool fromHH = false)
		{
			if (GameController != null)
			{
				var shaderData = (GameController as GameUIManager)._currentSharedData;
				var pl = shaderData.GetPlayer(UserId);
				//if (pl != null && pl.Combinations.Count > 0)
				//	_winCombinationPlayer = _shredDataPlayer.Combinations;
			}

			//else if (_shredDataPlayer.combinations.Count > 0)
			//	_winCombinationsPlayer = _shredDataPlayer.combinations;

			//_distributionsCards = _distributionsCards;
			//ClearState();

			if (_shredDataPlayer == null) return;

			Init(_shredDataPlayer.user, _shredDataPlayer.amount, TablePlayerSession != null ? TablePlayerSession.amountBuffer : 0);
			if (_distributionsCards != null && _distributionsCards.Count > 0)
				InitCards(_distributionsCards, _isShowDown, fromHH);
			bool isMe = _shredDataPlayer.user.id == UserController.User.id;
			_timer.IsMe = isMe;

			if (_straddlePanel.gameObject.activeSelf)
				_straddleLabel.anchoredPosition = new Vector2(0, isMe ? -1.5f : 0);

			if (!_WinSee && (GameController == null || !GameController.GamePanel.GameSession.WainWinFlag))
				foreach (var elem in _cards)
					elem.ForceNoWin();

			if (_action != null && _action.user_id == _shredDataPlayer.user.id)
			{
				_activeLight.gameObject.SetActive(true);
				if (!string.IsNullOrEmpty(_action.calltime_at))
				{
					var endTime = DateTime.Parse(_sharedData.active_event.calltime_at);
					var diff = (endTime - GameHelper.NowTime).TotalSeconds;
					if (diff > 0)
					{
						if (_timer.IsMe)
							if (!AppConfig.DisableAudio)
								DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_CHAT, 1, null, 0, StringConstants.SOUND_CHAT_RECIEVE);
						StartTimer(endTime);
					}
					else
					if (_timer.IsMe && GameController != null)
						GameController.GamePanel.EminUserAction(DateTime.MinValue);
				}
				else
				{
					it.Logger.Log("active_event.calltime_at == null");
				}
			}
			else
			{
				_activeLight.gameObject.SetActive(false);
				if (_timeLineRect != null)
					_timeLineRect.gameObject.SetActive(false);
				_timer.StopTimer();
				_timer.SetResetColor();
				if (_timer.IsMe && GameController != null)
					GameController.GamePanel.EminUserAction(DateTime.MinValue);
			}
			DistributionSharedDataPlayer player = null;
			if (_sharedData != null)
				for (int i = 0; i < _sharedData.players.Count; i++)
					if (_sharedData.players[i].user.id == _userId)
						player = _sharedData.players[i];

			ProcessAfk(_shredDataPlayer);
			// Ставим лейбл отсутствия за столом
			if (player != null)
			{
				_sittingOutLabel.gameObject.SetActive(player.is_resting || player.SkipDistributions);
				_balanceLabel.gameObject.SetActive((!player.is_resting && !player.SkipDistributions) && _shredDataPlayer.amount > 0);
				_allinLabel.gameObject.SetActive((!player.is_resting && !player.SkipDistributions) && _shredDataPlayer.amount <= 0);
			}
			else
			{
				_balanceLabel.gameObject.SetActive(true);
				_sittingOutLabel.gameObject.SetActive(false);
				_allinLabel.gameObject.SetActive(false);
			}

			if (_action != null)
				_lastActiveEventId = (int)_action.id;
		}

		public void OpenIfNeed()
		{
			if (_cards.Count <= 0) return;
			for (int i = 0; i < _distributionsCards.Count; i++)
			{
				if (!_distributionsCards[i].is_open.Value) continue;

				foreach (var card in _cards)
				{
					if (!card.Card.is_open.Value)
					{
						if (_distributionsCards[i].id == card.Card.id)
						{
							card.Init(_distributionsCards[i]);
							card.RotateToOpen();
						}
					}
				}


			}
		}

		public void ShowAction()
		{
			//if (_action != null && _lastActiveEventId != (int)_action.Id)
			InitState(_shredDataPlayer.ActiveStageStateName);
		}

		private void ProcessAfk(IMyAfk player)
		{

			if (player == null) return;

			if (!player.SkipDistributions && _realTimerAfk != null)
			{
				TimerManager.Instance.RemoveTimer(_realTimerAfk);
				_realTimerAfk.Stop();
				_realTimerAfk = null;
				_playerAfk.StopTimer();
				if (UserId == UserController.User.id && GameController != null)
					GameController.GamePanel.RequestMyAfk();
				_isAfk = false;

				com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.MyAfkActive, _isAfk, 0);
			}
			else if (player.SkipDistributions && _realTimerAfk == null)
			{
				_realTimerAfk = (TimerManager.RealTimer)TimerManager.Instance.GetTimer(StringConstants.AfkTimerName(UserId));

				if (_realTimerAfk == null)
					_realTimerAfk = (TimerManager.RealTimer)TimerManager.Instance.AddTimer(StringConstants.AfkTimerName(UserId), player.SkipDistributionsTime);
				_playerAfk.SetAfk(_realTimerAfk);
				_isAfk = true;
				com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.MyAfkActive, _isAfk, 0);
			}
			_balanceLabel.gameObject.SetActive(!_isAfk);
			_sittingOutLabel.gameObject.SetActive(_isAfk);
			_allinLabel.gameObject.SetActive(false);
			//_balanceLabel.gameObject.SetActive((!_shredDataPlayer.IsResting && !_shredDataPlayer.SkipDistributions) && _shredDataPlayer.Amount > 0);
			//_allinLabel.gameObject.SetActive((!_shredDataPlayer.IsResting && !_shredDataPlayer.SkipDistributions) && _shredDataPlayer.Amount <= 0);

		}

		public TimerManager.RealTimer GetStateAfk()
		{
			return _realTimerAfk;
		}


		public void SetUseTimeBank()
		{
			_timer.SetRedColor();
		}

		public bool ExistsLowCombinations()
		{
			List<DistributionPlayerCombination> lComb = _sharedData.MaxLowCombintion();
			//List<DistributionPlayerCombination> hComb = _sharedData.MaxHightCombintion();

			//if (hComb != null && lComb != null /*&& hComb.Equalence(lComb)*/)
			//	return false;

			foreach (var combination in _shredDataPlayer.combinations)
			{
				if ((GameType.OmahaLow5 == (GameType)GameController.SelectTable.game_rule_id || GameType.OmahaLow4 == (GameType)GameController.SelectTable.game_rule_id)
			&& combination.category == "low")
				{
					if (lComb == null || !lComb.Exists(x =>/*x.worth == combination.worth && */x.id == combination.id)) continue;
					//var commLow = _sharedData.MaxLowCombintion();

					return true;
				}
			}
			return false;
		}

		public bool ExistsCombinations()
		{
			List<DistributionPlayerCombination> hComb = _sharedData.MaxHightCombintion();
			foreach (var combination in _shredDataPlayer.combinations)
			{
				bool existsCombinations = false;

				foreach (var c in _cards)
					if (combination.IsContainsCard(c.Card.card.id))
						existsCombinations = true;

				if (!existsCombinations) continue;

				if ((combination.category == null || combination.category != "low"))
				{
					if (hComb == null || !hComb.Exists(x =>/* x.worth == combination.worth && */x.id == combination.id)) continue;
					//var commHigh = _sharedData.MaxHightCombintion();
					//if (commHigh == null || commHigh.Id != combination.Id) continue;
					return true;
				}
			}
			return false;
		}

		public void ShowCloneLowCombinations()
		{
			if (!(_shredDataPlayer.IsWin && _WinSee))
			{
				return;
			}
			bool existsHi = ExistsCombinations();
			bool lowCombination = false;
			foreach (var combination in _shredDataPlayer.combinations)
			{

				if (!lowCombination && (GameType.OmahaLow5 == (GameType)GameController.SelectTable.game_rule_id
				|| GameType.OmahaLow4 == (GameType)GameController.SelectTable.game_rule_id)
				&& combination.category == "low")
				{
					bool existsCombinations = false;

					foreach (var c in _cards)
						if (combination.IsContainsCard(c.Card.card.id))
							existsCombinations = true;

					if (!existsCombinations) continue;

					lowCombination = true;
					Instantiate(_parentWinCombination.gameObject, _parentWinCombination.parent);

					_winCombinations.ForEach(x => x.Image.SetActive(false));
					if (existsHi)
						_parentWinCombination.anchoredPosition = new Vector2(_parentWinCombination.anchoredPosition.x, _parentWinCombination.anchoredPosition.y - _parentWinCombination.rect.height * 2);

					for (int i = 0; i < _winCombinations.Count; i++)
					{
						if (_winCombinations[i].Name.ToLower() == combination.game_card_combination.slug.ToLower())
						{
							_winBlock.gameObject.SetActive(true);
							_parentWinCombination.gameObject.SetActive(true);
							_winCombinations[i].Image.SetActive(true);
							//break;
						}
					}
				}
			}
		}

		public void GetCombinationsActiveStage()
		{
			if ((UserId != UserController.User.id)
				|| (_shredDataPlayer == null || _combinationLabel == null)
				|| (_sharedData == null || !_sharedData.ExistsFlop()))
			{
				ClearCombinations();
				return;
			}

			//if (UserId != UserController.User.id) return;
			//if (_shredDataPlayer == null || _combinationLabel == null) return;
			//if (_sharedData == null || !_sharedData.ExistsFlop()) return;

			if (GameController == null)
			{
				it.Logger.LogError("GameController null");
				return;
			}
			if (GameController.SelectTable == null)
			{
				it.Logger.LogError("GameController.SelectTable null");
				return;
			}

			TableApi.GetCombination(GameController.SelectTable.id, (result) =>
			{
				_combinationLabel.text = (result.IsSuccess && result.Result.high != null) ? result.Result.high.title : "";
				if (_combinationLowLabel != null)
					_combinationLowLabel.text = (result.IsSuccess && result.Result.low != null) ? result.Result.low.title : "";
			});

		}

		public void ClearCombinations()
		{
			_combinationLabel.text = "";
			_combinationLowLabel.text = "";
		}

		public void ShowCombinations(List<DistributionPlayerCombination> winCombinations)
		{

			//if (!(_shredDataPlayer.IsWin && _WinSee))
			//{
			//	_parentWinCombination.gameObject.SetActive(false);
			//	return;
			//}
			foreach (var c in _cards)
				if (c.Card.IsFolded)
					return;

			foreach (var combination in winCombinations)
			{
				bool existsCombinations = false;
				foreach (var c in _cards)
					if (c.Card.card != null && combination.IsContainsCard(c.Card.card.id))
						existsCombinations = true;
				if (!existsCombinations)
					continue;

				_winCombinations.ForEach(x => x.Image.SetActive(false));

				for (int i = 0; i < _winCombinations.Count; i++)
				{
					if (_winCombinations[i].Name.ToLower() == combination.game_card_combination.slug.ToLower())
					{
						_winBlock.gameObject.SetActive(true);
						_parentWinCombination.gameObject.SetActive(true);
						_winCombinations[i].Image.SetActive(true);

					}
				}
			}

		}

		public void ShowCombinations(bool lowAnimate = true)
		{
			if (GameController != null && GameController.GamePanel.GameSession.WainWinFlag) return;

			if (!(_shredDataPlayer.IsWin && _WinSee))
			{
				_parentWinCombination.gameObject.SetActive(false);
				return;
			}

			float _timeDelay = 2;

			List<DistributionPlayerCombination> hComb = _sharedData.MaxHightCombintion();
			List<DistributionPlayerCombination> lComb = _sharedData.MaxLowCombintion();

			bool hiCombination = false;
			bool lowCombination = false;

			if (!_WinSee) return;

			//else
			//	NoWin();
			//_WinSee = false;
			foreach (var combination in _winCombinationPlayer)
			{
				if (lowAnimate && !lowCombination
				&& (GameType.OmahaLow5 == (GameType)GameController.SelectTable.game_rule_id || GameType.OmahaLow4 == (GameType)GameController.SelectTable.game_rule_id)
				&& combination.category == "low")
				{
					if (lComb == null || !lComb.Exists(x => x.worth == combination.worth && x.id == combination.id)) continue;

					bool existsLowCombinations = false;

					foreach (var c in _cards)
						if (combination.IsContainsCard(c.Card.card.id))
							existsLowCombinations = true;
					if (!existsLowCombinations)
						continue;

					lowCombination = true;
					_delayLowCombination = DG.Tweening.DOVirtual.DelayedCall(_timeDelay, () =>
					{
						_winCombinations.ForEach(x => x.Image.SetActive(false));

						for (int i = 0; i < _winCombinations.Count; i++)
						{
							if (_winCombinations[i].Name.ToLower() == combination.game_card_combination.slug.ToLower())
							{
								_winBlock.gameObject.SetActive(true);
								_parentWinCombination.gameObject.SetActive(true);
								_winCombinations[i].Image.SetActive(true);
								//break;
							}
						}
					});
				}


				if (!hiCombination && (combination.category == null || combination.category != "low"))
				{
					if (hComb == null || !hComb.Exists(x => x.worth == combination.worth && x.id == combination.id)) continue;

					bool existsCombinations = false;
					foreach (var c in _cards)
						if (combination.IsContainsCard(c.Card.card.id))
							existsCombinations = true;
					if (!existsCombinations)
						continue;

					_winCombinations.ForEach(x => x.Image.SetActive(false));


					for (int i = 0; i < _winCombinations.Count; i++)
					{

						if (_winCombinations[i].Name.ToLower() == combination.game_card_combination.slug.ToLower())
						{
							_winBlock.gameObject.SetActive(true);
							_parentWinCombination.gameObject.SetActive(true);
							_winCombinations[i].Image.SetActive(true);
							hiCombination = true;

							if (lComb != null && lowAnimate)
								DOVirtual.DelayedCall(_timeDelay, () =>
								{
									NoWin();
								});

						}
					}


				}
			}
		}


		private void InitCombinations(bool winSee, List<DistributionPlayerCombination> combinations)
		{
			if (!winSee)
			{
				_parentWinCombination.gameObject.SetActive(false);
				return;
			}

			if (combinations.Count > 0)
			{
				_winCombinationPlayer = combinations;
				//ShowCombitaions();
			}
			else
			{
				_parentWinCombination.gameObject.SetActive(false);
			}
		}

		public void StartTimer(DateTime dateTime)
		{
			if (_timeLineRect != null)
				_timeLineRect.gameObject.SetActive(true);
			_timer.StartTimer(dateTime);
			if (_timer.IsMe && GameController != null)
				GameController.GamePanel.EminUserAction(dateTime);

		}

		public void ClearState()
		{
			if (_timer == null) return;
			_timer.StopTimer();
			if (GameController != null)
				GameController.GamePanel.EminUserAction(DateTime.MinValue);
			if (_timeLineRect != null)
				_timeLineRect.gameObject.SetActive(false);
			_stateContainer.gameObject.SetActive(false);
			//_cards.ForEach(x => Destroy(x.gameObject));
			//_cards.Clear();
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS

			//WinStateImage.SetActive(false);
#endif
		}

		public void ClearCards()
		{
			//_cards.ForEach(x => Destroy(x.gameObject));

			_cards.ForEach(x =>
			{
				PoolerManager.Return("GameCard", x.gameObject);
			});
			_cards.Clear();
		}

		public void OpenCards()
		{
			//CardsContainer.SetActive(true);
			//ClosedCardsContainer.SetActive(false);

			foreach (var card in _cards)
				card.RotateToOpen();

		}
		public void ClearStateWithoutWin()
		{
			if (_timeLineRect != null)
				_timeLineRect.gameObject.SetActive(false);
			_timer.StopTimer();
			if (_timer.IsMe && GameController != null)
				GameController.GamePanel.EminUserAction(DateTime.MinValue);

		}
		public void ShowWin()
		{
			if (_cards.Count <= 0) return;
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS

			// WinStateImage.SetActive(true);
#endif
			_winBlock.gameObject.SetActive(_WinSee);
			//_stateContainer.gameObject.SetActive(dataPlayer.IsWin);
			if (_shredDataPlayer.IsWin)
			{
				if (_timeLineRect != null)
					_timeLineRect.gameObject.SetActive(false);
				_timer.StopTimer();
			}

		}
		public void NoWin()
		{
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS

			// WinStateImage.SetActive(true);
#endif
			_winCombinations.ForEach(x => x.Image.SetActive(false));
			_WinSee = false;
			_winBlock.gameObject.SetActive(_WinSee);
			_parentWinCombination.gameObject.SetActive(false);

		}

		void Init(UserLimited user, decimal amount, decimal amountBuffer, bool isShowDown = false)
		{
			this.user = user;
			_nickname.text = user.nickname;
			_balanceLabel.SetValue(amount);
			_sittingOutLabel.gameObject.SetActive(_shredDataPlayer.is_resting || _shredDataPlayer.SkipDistributions);
			_balanceLabel.gameObject.SetActive((!_shredDataPlayer.is_resting && !_shredDataPlayer.SkipDistributions) && _shredDataPlayer.amount > 0);
			_allinLabel.gameObject.SetActive((!_shredDataPlayer.is_resting && !_shredDataPlayer.SkipDistributions) && _shredDataPlayer.amount <= 0);
			//amountBuffer

			//PlayerInfoPanelUI.Init(user, amount, amountBuffer);

			SetFlag(user.country.flag);


			//_defaultAvatar.gameObject.SetActive(true);
			if (!string.IsNullOrEmpty(user.AvatarUrl))
			{
				_avatar.SetDefaultAvatar();
				_avatar.SetAvatar(user.AvatarUrl);
			}
			if (TablePlayerSession != null && TablePlayerSession.user != null && TablePlayerSession.user.user_profile != null && TablePlayerSession.user.user_profile.avatarUrl.Length > 0)
				_avatar.SetAvatar(TablePlayerSession.user.user_profile.avatarUrl);

			//CardsContainer.SetActive(isShowDown);
			//ClosedCardsContainer.SetActive(false);
		}

		public void SetFlag(string url)
		{

			if (string.IsNullOrEmpty(url) || _flag == null) return;

			it.Managers.NetworkManager.Instance.RequestTexture(url, (s, b) =>
			{
				if (gameObject == null || _flag == null) return;
				_flag.texture = s;
				_flag.gameObject.SetActive(true);
			}, (err) =>
			{
				if (gameObject == null || _flag == null) return;
				_flag.gameObject.SetActive(false);
			});

		}

		public void InitState(string state, bool timeDisable = true)
		{
			_stateContainer.gameObject.SetActive(true);
			bool existsState = false;

			if (state == "")
			{
				if (!_waitStateLabel)
					_stateContainer.gameObject.SetActive(false);
				//if (_waitstate != null)
				//	StopCoroutine(_waitstate);
				//_stateContainer.gameObject.SetActive(false);
				return;
			}
			if (state == "fold")
			{
				existsState = true;
				//CardsContainer.SetActive(false);
				//ClosedCardsContainer.SetActive(false);
				//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
				//				for (int i = 0; i <CardPlaces.Count; i++)
				//				CardPlaces[i].gameObject.SetActive(false);
				//#endif
			}

			for (int i = 0; i < _stateList.Count; i++)
			{
				if (_stateList[i].Name == state)
				{
					existsState = true;
					_stateContainer.color = _stateList[i].Color;
					_stateText.color = _stateList[i].Color;
					_stateText.text = _stateList[i].Title;
					break;
				}

			}

			if (existsState && state != "win" && timeDisable)
			{
				if (_waitstate != null)
					StopCoroutine(_waitstate);
				_waitstate = StartCoroutine(CoroutoneState());
				return;
			}
			if (!existsState)
				_stateContainer.gameObject.SetActive(false);

		}
		private Coroutine _waitstate;
		private bool _waitStateLabel;
		IEnumerator CoroutoneState()
		{
			_waitStateLabel = true;
			yield return new WaitForSeconds(2);
			_waitStateLabel = false;
			_stateContainer.gameObject.SetActive(false);
			InitState("");
		}
		public void ShowDropCards(List<DistributionCard> distributionCards)
		{
			try
			{
				WaitToClickToShowShow = true;
				InitCards(distributionCards, false);
				_cards.ForEach(x => x.SetGrayColor());
			}
			catch (Exception ex)
			{
				it.Logger.LogError("ShowDropCards error " + ex.Message);
			}
		}

		private void InitCards(List<DistributionCard> distributionCards, bool isShowDown, bool isHandHistory = false)
		{
			//if (GameController == null) return;

			//it.Logger.Log("InitCards");


			var isShowCard = user.id == GameHelper.UserInfo.id || isShowDown;
			if (VisibleDropCards) return;

			bool isFold = _shredDataPlayer.state.slug == "fold";
			bool isOpenFold = false;
			for (int i = 0; i < distributionCards.Count; i++)
			{
				if (distributionCards[i].IsFolded && (bool)distributionCards[i].is_open && WaitToClickToShowShow)
				{
					VisibleDropCards = true;
					isOpenFold = true;
					WaitToClickToShowShow = false;
				}
			}
			if (GameController != null && _sharedData != null /*&& VisibleDropCards*/)
				if (!isHandHistory && _sharedData.IsFinish && !isOpenFold)
					return;

			it.Logger.Log("[PlayerGameIcone] InitCards user id " + UserId);

			//if (GameController != null && GameController.GamePanel.GameSession.CardOutPlayers.Contains(UserId) && !isOpenFold)
			//	return;

			_cardPlaces = _cardsPositions.Cards.Find(x => x.CardsCount == distributionCards.Count).CardPositions;

			if (WaitMovePlayerAnimation) return;

			for (int i = 0; i < distributionCards.Count; i++)
			{
				var cardItem = _cards.Find(x => x.Card.id == distributionCards[i].id);

				isFold = ((bool)distributionCards[i].IsFolded || isFold) && !isHandHistory;
				bool isOpen = (bool)distributionCards[i].is_open /*|| isOpenFold*/;
				bool isVisible = (!isFold || isOpenFold) || isOpen;

				if (cardItem == null)
				{
					GameObject inst = PoolerManager.Spawn("GameCard");
					inst.transform.SetParent(_cardPlaces[i]);
					inst.transform.localScale = Vector3.one;
					inst.transform.localPosition = Vector3.zero;
					RectTransform cardRT = inst.transform.GetComponent<RectTransform>();
					cardRT.anchoredPosition = Vector3.zero;
					cardRT.localRotation = Quaternion.identity;
					cardRT.localScale = Vector3.one;
					cardRT.anchorMin = Vector2.zero;
					cardRT.anchorMax = Vector2.one;
					cardRT.sizeDelta = Vector2.zero;
					cardRT.localPosition = Vector2.zero;
					cardItem = inst.GetComponent<GameCardUI>();
					cardItem.ClearState();
					_cards.Add(cardItem);
					cardItem.IsPlayerCard = true;
					cardItem.SetCloseState();
				}

				cardItem.Init(distributionCards[i]);

				//if (distributionCards[i].IsFolded && !(bool)distributionCards[i].is_open && !isHandHistory)
				//	cardItem.SetVisible(false);

				//if (isFold && isOpenFold)
				//	cardItem.SetVisible(true);

				//if (user.id != GameHelper.UserInfo.id || (bool)distributionCards[i].IsFolded)
				//	cardItem.SetCloseState();

				//if ((isShowCard && !(bool)distributionCards[i].IsFolded) || (bool)distributionCards[i].is_open)
				//	cardItem.SetOpenState();

				cardItem.SetVisible(isVisible);

				if ((IsMe && (!isFold || isShowCard)) || isOpen)
					cardItem.SetOpenState();
				else
					cardItem.SetCloseState();

				if (VisibleDropCards)
					cardItem.SetGrayColor();

			}
			if (isShowCard && _WinSee && (GameController == null || !GameController.GamePanel.GameSession.WainPlayerCombitaions) && !isFold)
				ShowCardsCombinations();

		}
		public void ShowCardsCombinations(List<DistributionPlayerCombination> combination)
		{
			for (int i = 0; i < _distributionsCards.Count; i++)
			{
				var card = _distributionsCards[i];
				var cardItem = Cards.Find(x => x.Card.id == card.id);

				if (cardItem == null) continue;

				if (_distributionsCards[i].IsFolded)
				{
					cardItem.SetGrayColor();
					continue;
				}

				bool isClose = cardItem.IsClose;

				cardItem.Init(card, combination, combination, _shredDataPlayer.IsWin, false, true);
				if (isClose)
				{
					cardItem.SetCloseState();
					if ((bool)_distributionsCards[i].is_open && cardItem.IsClose)
						cardItem.RotateToOpen();
				}

			}
		}
		public void ShowCardsCombinations()
		{
			if (_sharedData == null || !_sharedData.IsFinish) return;
			for (int i = 0; i < _distributionsCards.Count; i++)
			{

				var card = _distributionsCards[i];
				var cardItem = Cards.Find(x => x.Card.id == card.id);


				if (cardItem == null) continue;

				if (_distributionsCards[i].IsFolded)
				{
					cardItem.SetGrayColor();
					continue;
				}

				bool isClose = cardItem.IsClose;

				cardItem.Init(card, _shredDataPlayer.combinations, _winCombinationPlayer, _shredDataPlayer.IsWin);
				if (isClose)
				{
					cardItem.SetCloseState();
					if ((bool)_distributionsCards[i].is_open && cardItem.IsClose)
						cardItem.RotateToOpen();
				}

			}
			//_winCombinationPlayer.Clear();
		}


		public void ShowInfo()
		{
			if (TablePlayerSession == null || TablePlayerSession.user == null || (GameController != null && GameController.SelectTable.is_all_or_nothing)) return;
			if (GameController != null)
			{
				if (_shredDataPlayer != null)
					GameController.GamePanel.OpenUserInfo(TablePlayerSession.user, _shredDataPlayer.user_stat, _note);
				else
					GameController.GamePanel.OpenUserInfo(TablePlayerSession.user, TablePlayerSession.user_stat, _note);
			}
		}

		Tween _delayLightInfo;
		private bool _isLightPanel;
		public void ShowLighInfo()
		{
#if !UNITY_STANDALONE
			return;
#endif


			if (TablePlayerSession == null || TablePlayerSession.user == null || TablePlayerSession.user.id == UserController.User.id || (GameController != null && GameController.SelectTable.is_all_or_nothing)) return;

			_delayLightInfo = DOVirtual.DelayedCall(1, () =>
			{
				_isLightPanel = true;
				var panel = it.Main.PopupController.Instance.ShowPopup<SmartHudOpponentLightPopup>(PopupType.SmartHudLightOpponenty, true);
				//panel.SetData(TablePlayerSession.Amount, TablePlayerSession.User, TablePlayerSession.UserStat, _note);
				if (_shredDataPlayer != null)
					panel.SetData(TablePlayerSession.amount, TablePlayerSession.user, _shredDataPlayer.user_stat, _note);
				else
					panel.SetData(TablePlayerSession.amount, TablePlayerSession.user, TablePlayerSession.user_stat, _note);
			});

		}
		public void HideLighInfo()
		{
			if (TablePlayerSession == null || TablePlayerSession.user == null || TablePlayerSession.user.id == UserController.User.id) return;
			if (_delayLightInfo != null && _delayLightInfo.active)
			{
				_isLightPanel = false;
				_delayLightInfo.Kill();
				return;
			}

			it.Main.PopupController.Instance.ClosePopup(PopupType.SmartHudLightOpponenty);
		}

	}

	/// <summary>
	/// Отображаемый диалог у пользователя
	/// </summary>
	public class DialogMessages : MonoBehaviour
	{
		private RectTransform _backRect;
		private TextMeshProUGUI _label;
		private RectTransform _rt;
		private Vector2 _startRect;
		private bool _isVisible;
		private Tween _tweenHide;

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			_rt = gameObject.GetComponent<RectTransform>();
			_backRect = transform.Find("Back").GetComponent<RectTransform>();
			_label = transform.Find("Label").GetComponent<TextMeshProUGUI>();
			_startRect = _rt.sizeDelta;
			_rt.sizeDelta = Vector2.zero;
		}

		public void SetMessage(bool isLeft, string message)
		{
			gameObject.SetActive(true);
			_label.text = message;

			float hideAnimPosition = 0;
			if (_tweenHide != null && _tweenHide.active)
			{
				hideAnimPosition = _tweenHide.position;
				_tweenHide.Kill();
			}

			Vector2 targetSize = new Vector2(_startRect.x, Mathf.Min(_label.preferredHeight + 20, _startRect.y));
			_rt.DOSizeDelta(targetSize, 0.2f - hideAnimPosition).OnComplete(() =>
			{
				_label.color = Color.white;
			});
			_backRect.localScale = new Vector3((isLeft ? -1 : 1), 1, 1);
			_rt.pivot = new Vector2((isLeft ? 1 : 0), _rt.pivot.y);

			_tweenHide = _rt.DOSizeDelta(Vector2.zero, 0.2f).SetDelay(5).OnStart(() =>
			{
				_label.color = Color.clear;
			}).OnComplete(() =>
			{
				gameObject.SetActive(false);
			});

		}

	}

}