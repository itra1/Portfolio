using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using it.Network.Rest;
using DG.Tweening;

public class GameCardUI : MonoBehaviour
{
	public DistributionCard GetInfoCard => _card;

	public UnityEngine.Events.UnityAction<bool> OnIsMoveUp;

	public bool UpdateProfile = true;
	public bool IsClose { get; set; } = false;
	public bool IsMini = false;
	public bool ShodowMove = false;
	public bool IsPlayerCard;
	public bool IsOnTable;
	public Image Shadow;
	[SerializeField] private Image _bingoLight;
	[SerializeField] private Image cardSpriteRenderer;
	[SerializeField] private Image shineSpriteRenderer;
	[SerializeField] private RectTransform rectTransform;
	[SerializeField] private Color colorShadow;
	[SerializeField] private DistributionCard _card;
	[SerializeField] private RectTransform _bodyRect;

	[HideInInspector] public bool isShowWin;
	//public bool IsEmitMoveUpEvent;
	private Tween _animateWinColor;
	private Tween _animateWinMove;
	private Tween _activeHight;
	private Tween _delayLow;
	private Tween _rotate;


	private it.UI.GamePanel _gamePanel;

	public it.UI.GamePanel GamePanel
	{
		get
		{
			if (_gamePanel == null)
				_gamePanel = GetComponentInParent<it.UI.GamePanel>();
			return _gamePanel;
		}
	}

	public DistributionCard Card { get => _card; set => _card = value; }
	//public RectTransform BodyRect { get => _bodyRect; set => _bodyRect = value; }
	public Image BingoLight { get => _bingoLight; set => _bingoLight = value; }
	public Image ShineSpriteRenderer { get => shineSpriteRenderer; set => shineSpriteRenderer = value; }

	public void ClearState()
	{
		SetVisible(true);
	}

	public void SetVisible(bool isVisible)
	{
		_bodyRect.gameObject.SetActive(isVisible);
	}

	private void Start()
	{
		if (UpdateProfile)
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
	}

	private void OnEnable()
	{
		_bingoLight.gameObject.SetActive(false);
		if (IsClose)
			SetCloseState();
	}

	private void OnDisable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
		_bodyRect.gameObject.SetActive(true);
		ForceNoWin();
	}

	//private void LateUpdate()
	//{
	//	if (ShodowMove)
	//	{
	//		MoveShadow();
	//	}
	//}
	//public void MoveShadow()
	//{
	//	//Shadow.rectTransform.localPosition = /*cardSpriteRenderer.rectTransform.position +*/ Vector3.down * 10f /* * transform.localScale.y*/;
	//}

	private void UserProfileUpdate(com.ootii.Messages.IMessage handle)
	{
		ConfirmSprite();
	}

	public void SetOpenState(bool fromRotateAnimation = false)
	{
		if (_card == null || _card.card == null) return;

		//if (!IsClose) return;

		if (!fromRotateAnimation)
			ResetRotateAnimation();

		IsClose = false;
		cardSpriteRenderer.sprite = IsMini ? CardLibrary.Instance.GetCardMini(_card) : CardLibrary.Instance.GetCard(_card);
	}

	public void SetCloseState(bool fromRotateAnimation = false)
	{
		//if (IsClose) return;
		IsClose = true;

		if (!fromRotateAnimation)
			ResetRotateAnimation();

		cardSpriteRenderer.sprite = CardLibrary.Instance.GetCardBack();
	}

	public void RotateToOpen()
	{
		if (!IsClose)
		{
			SetOpenState();
			return;
		}
		IsClose = false;
		Debug.Log("Card Rotate To Open");

		ResetRotateAnimation();

		float time = 0.15f;
		bool waitCenter = true;
		Vector3 cardScale = cardSpriteRenderer.transform.localScale;
		_rotate = _bodyRect.DOLocalRotate(new Vector3(0, -180, 0), time).OnUpdate(() =>
		{
			if (waitCenter)
			{
				if (_bodyRect.localEulerAngles.y <= 270)
				{
					SetOpenState(true);
					cardSpriteRenderer.transform.localScale = new Vector3(-cardScale.x, cardScale.y, cardScale.z);
					waitCenter = false;
				}
			}
		}).OnComplete(() =>
		{
			_bodyRect.localEulerAngles = Vector3.zero;
			cardSpriteRenderer.transform.localScale = cardScale;
		});
	}

	public void RotateToClose()
	{
		if (IsClose)
		{
			SetCloseState();
			return;
		}
		IsClose = true;
		Debug.Log("Card Rotate To Close");

		ResetRotateAnimation();

		float time = 0.15f;
		bool waitCenter = true;
		Vector3 cardScale = cardSpriteRenderer.transform.localScale;
		_rotate = _bodyRect.DOLocalRotate(new Vector3(0, 180, 0), time).OnUpdate(() =>
		{
			if (waitCenter)
			{
				if (_bodyRect.localEulerAngles.y >= 90)
				{
					SetCloseState(true);
					cardSpriteRenderer.transform.localScale = new Vector3(-cardScale.x, cardScale.y, cardScale.z);
					waitCenter = false;
				}
			}
		}).OnComplete(() =>
		{
			_bodyRect.localEulerAngles = Vector3.zero;
			cardSpriteRenderer.transform.localScale = cardScale;
		});
	}

	public void Init(DistributionCard card)
	{

		this._card = card;
		//IsClose = false;
		//ConfirmSprite();
	}

	public void Init(DistributionCard card, List<DistributionPlayerCombination> myCombinations, List<DistributionPlayerCombination> allCombinations, bool isWin = true, bool isChina = false, bool history = false)
	{
		Init(card);
		if (history)
			ApplyCombinationOnly(myCombinations, allCombinations, isWin, isChina);
		else
			ApplyCombination(myCombinations, allCombinations, isWin, isChina);
	}
	public void Init(List<DistributionPlayerCombination> myCombinations, List<DistributionPlayerCombination> allCombinations, bool isWin = true, bool isChina = false, bool history = false)
	{
		if (history)
			ApplyCombinationOnly(myCombinations, allCombinations, isWin, isChina);
		else
			ApplyCombination(myCombinations, allCombinations, isWin, isChina);
	}

	private void ConfirmSprite()
	{
		if (IsClose)
			SetCloseState();
		else
			cardSpriteRenderer.sprite = IsMini ? CardLibrary.Instance.GetCardMini(_card) : CardLibrary.Instance.GetCard(_card);
	}

	public void InitOnlyVisual(DistributionCard card)
	{
		this._card = card;
		cardSpriteRenderer.sprite = IsMini ? CardLibrary.Instance.GetCardMini(card) : CardLibrary.Instance.GetCard(card);
	}

	public void ApplyCombinationOnly(List<DistributionPlayerCombination> myCombinations, List<DistributionPlayerCombination> allCombinations, bool isWin, bool isChina)
	{

		//if (myCombinations.Count == 0)
		//{
		//if (!IsOnTable && allCombinations.Count > 0)
		//	SetCombinationState(false, isWin, isChina);
		//	return;
		//}
		bool exists = false;
		if (allCombinations != null && allCombinations.Count > 0)
		{
			foreach (var combination in myCombinations)
			{
				if (!exists)
				{
					//if (combination.IsContainsCard(_card.CardId))
					//	exists = true;
					SetCombinationState(combination.IsContainsCard(_card.CardId), isWin, isChina);
				}
			}
		}
		else
		{
			cardSpriteRenderer.color = Color.white;
			shineSpriteRenderer.gameObject.SetActive(false);
			_bodyRect.localPosition = Vector3.zero;
		}
	}

	[Obsolete]
	private void ApplyCombination(List<DistributionPlayerCombination> myCombinations, List<DistributionPlayerCombination> allCombinations, bool isWin, bool isChina)
	{

		//if (myCombinations.Count == 0)
		//{
		if (!IsOnTable && allCombinations.Count > 0)
			SetCombinationState(false, isWin, isChina);
		//	return;
		//}

		if (allCombinations != null && allCombinations.Count > 0)
		{
			var lcomb = DistributionPlayerCombination.MaxLowCombintion(allCombinations);
			var hcomb = DistributionPlayerCombination.MaxHightCombintion(allCombinations);

			bool hiCombination = false;
			bool lowCombination = false;

			foreach (var combination in myCombinations)
			{
				try
				{
					if (!lowCombination && isWin
					&& (GameType.OmahaLow5 == (GameType)GamePanel.CurrentGameUIManager.SelectTable.game_rule_id || GameType.OmahaLow4 == (GameType)GamePanel.CurrentGameUIManager.SelectTable.game_rule_id)
					&& combination.category == "low"
					&& lcomb.Exists(x => x.id == combination.id))
					{
						var c1 = combination.IsContainsCard(_card.CardId);
						var c2 = isWin;
						var c3 = isChina;
						lowCombination = true;
						if (_activeHight != null && _activeHight.IsActive())
							_activeHight.Kill();
						_delayLow = DG.Tweening.DOVirtual.DelayedCall(1.5f, () =>
					{
						SetCombinationState(c1, c2, c3);
					});
					}
				}
				catch (Exception ex)
				{
					it.Logger.Log(ex.Message);
				}

				if (!hiCombination && (combination.category == null || combination.category != "low") && hcomb.Exists(x => x.id == combination.id))
				{
					if (combination.IsContainsCard(_card.CardId))
						hiCombination = true;
					SetCombinationState(combination.IsContainsCard(_card.CardId), isWin, isChina);
					_activeHight = DG.Tweening.DOVirtual.DelayedCall(1.5f, () =>
					{
						if (!lowCombination) return;
						SetCombinationState(false, isWin, isChina);
					});
					//break;
				}
			}
		}
		else
		{
			cardSpriteRenderer.color = Color.white;
			shineSpriteRenderer.gameObject.SetActive(false);
			_bodyRect.localPosition = Vector3.zero;
		}
	}

	//Tween _waitToShowWin;
	private void SetCombinationState(bool isCombinate, bool isWin, bool isChina)
	{
		float _time = 0.1f;

		// Временная задержка после показа выграшной комбинации на столе в 1,5 с
		//if (IsPlayerCard && GamePanel != null)
		//{
		//	double delta = Time.timeAsDouble - GamePanel.GameSession.ShowWinCardTableTime;
		//	if (delta < 1.5f)
		//	{

		//		if (_waitToShowWin != null && _waitToShowWin.active)
		//			_waitToShowWin.Kill();

		//		GamePanel.GameSession.ShowWinCardPlayerTime = double.MaxValue;
		//		_waitToShowWin = DOVirtual.DelayedCall((float)(1.5 - delta), () =>
		//		{
		//			SetCombinationState(isCombinate, isWin, isChina);
		//		});
		//		return;
		//	}
		//}
		if (GamePanel != null)
			GamePanel.GameSession.ShowWinCardPlayerTime = Time.timeAsDouble;

		isShowWin = isCombinate && isWin;
		shineSpriteRenderer.gameObject.SetActive(isShowWin);

		//if (!isShowWin)
		//	cardSpriteRenderer.color = Color.white;

		Color targetColor = !isShowWin ? Color.gray : Color.white;
		//cardSpriteRenderer.color = !isShowWin ? Color.gray : Color.white;

		if (cardSpriteRenderer.color != targetColor)
			_animateWinColor = cardSpriteRenderer.DOColor(targetColor, _time);

		if (!isChina)
		{
			if (isShowWin)
			{
				//if (IsEmitMoveUpEvent)
				//	com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.CardsMoveUpGameCombitation);
				OnIsMoveUp?.Invoke(true);
				_animateWinMove = _bodyRect.DOLocalMove(new Vector2(0, 15), _time);
				_animateWinColor = cardSpriteRenderer.DOColor(Color.white, _time);
			}
			else
			{
				_bodyRect.localPosition = new Vector2(0, 0);
				//_animateWinColor = cardSpriteRenderer.DOColor(Color.gray, 0.3f);
			}
		}
	}

	public void SetGrayColor()
	{
		cardSpriteRenderer.color = Color.gray;
	}

	public void NoWin()
	{
		OnIsMoveUp?.Invoke(false);
		if (_animateWinColor != null)
			_animateWinColor.Kill();
		_animateWinColor = cardSpriteRenderer.DOColor(Color.white, 0.3f);

		if (_animateWinMove != null)
			_animateWinMove.Kill();
		_animateWinMove = _bodyRect.DOLocalMove(new Vector2(0, 0), 0.3f);

		//cardSpriteRenderer.DOColor(Color.gray, 0.3f);
	}

	public void ForceNoWin()
	{
		OnIsMoveUp?.Invoke(false);
		if (_animateWinColor != null)
			_animateWinColor.Kill();
		cardSpriteRenderer.color = Color.white;

		if (_animateWinMove != null)
			_animateWinMove.Kill();
		_bodyRect.localPosition = new Vector2(0, 0);

		if (_delayLow != null && _delayLow.IsActive())
			_delayLow.Kill();

		//cardSpriteRenderer.DOColor(Color.gray, 0.3f);
	}

	public void SetSize(RectTransform distributionCardPlace)
	{
		rectTransform.sizeDelta = distributionCardPlace.sizeDelta;
	}

	private void ResetRotateAnimation()
	{
		//return;
		if (_rotate != null)
			_rotate.Kill(true);
		cardSpriteRenderer.transform.localScale = new Vector3(Mathf.Abs(cardSpriteRenderer.transform.localScale.x), Mathf.Abs(cardSpriteRenderer.transform.localScale.y), Mathf.Abs(cardSpriteRenderer.transform.localScale.z));
		_bodyRect.localEulerAngles = Vector3.zero;
	}

	public void SetBlockingState(bool isMeActive)
	{
		cardSpriteRenderer.color = isMeActive ? Color.white : colorShadow;
	}

}
