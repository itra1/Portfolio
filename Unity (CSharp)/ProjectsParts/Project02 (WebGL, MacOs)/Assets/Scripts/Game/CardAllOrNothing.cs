using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using it.Network.Rest;

public class CardAllOrNothing : MonoBehaviour
{
	[SerializeField] private GameObject _bingoLight;
	[SerializeField] private RectTransform _body;
	[SerializeField] private Image _image;
	[SerializeField] private Sprite _backSprite;
	[SerializeField] private Image _starImage;

	private RectTransform _rtImage;
	private Sprite _cardSprite;
	private bool _isVisible;
	private it.Network.Rest.Card _card;
	private bool _isAnimate = false;
	private bool _isMatch;
	private bool _isCard;

	public Card Card { get => _card; set => _card = value; }
	public GameObject BingoLight { get => _bingoLight; set => _bingoLight = value; }
	public Image StarImage { get => _starImage; set => _starImage = value; }

	[System.Flags]
	public enum Type { none = 0, card = 1, star = 2 };

	public void Init(it.Network.Rest.Card card, bool isMatch)
	{
		_rtImage = _image.GetComponent<RectTransform>();
		_isCard = false;
		if(!isMatch)
			_starImage.gameObject.SetActive(false);

		bool updateCard = _card == null || _card.id != card.id;
		bool updateIsMatch = _isMatch != isMatch;

		_isMatch = isMatch;
		_card = card;
		if (updateCard)
		{
			_cardSprite = CardLibrary.Instance.GetCardMicro(_card.card_type);

			if(_isVisible)
			{
				SetHideCard(() => { SetVisibleCard(null); });
			}else
			SetVisibleCard(null);

		}
		if(updateCard)
			ClearLight();

		if (updateIsMatch && isMatch)
		{
			StartCoroutine(VisibleStarCoroutone());
		}
	}

	public void ClearLight(){
		_bingoLight.gameObject.SetActive(false);
	}

	private IEnumerator VisibleStarCoroutone()
	{
		yield return new WaitForSeconds(0.5f);
		SetStarCard(null);
	}

	public void SetVisibleCard(UnityEngine.Events.UnityAction onComplete)
	{
		SetCardVisible(_cardSprite, onComplete);
		_isVisible = true;
	}
	public void SetHideCard(UnityEngine.Events.UnityAction onComplete)
	{
		SetCardVisible(_backSprite, ()=> {

			_isVisible = false;
			onComplete?.Invoke();
		});
	}
	public void SetStarCard(UnityEngine.Events.UnityAction onComplete)
	{
		Color w = Color.white;
		w.a = 0;
		_starImage.gameObject.SetActive(true);
		_starImage.color = w;
		_starImage.DOColor(Color.white, 0.4f).OnComplete(()=> {
			onComplete?.Invoke();
		});

		_isCard = true;
	}

	public void SetCardVisible(Sprite targetSprite, UnityEngine.Events.UnityAction onComplete)
	{
		bool wait = true;
		_isAnimate = true;

		var tw = DOTween.To(()=> _body.rotation, (x)=>_body.rotation = x, new Vector3(0, -180, 0),0.6f).OnComplete(() =>
		{
			_body.eulerAngles = new Vector3(0, 0, 0);
			_rtImage.transform.localScale = new Vector3(1, 1, 1);
			_isAnimate = false;
			DOVirtual.DelayedCall(0.1f, () => { onComplete?.Invoke(); });
		});

		tw.OnUpdate(() =>
		{
			if (!wait) return;
			if (tw.position >= 0.6f * 0.5f)
			{
				wait = false;
				_image.sprite = targetSprite;
				_rtImage.transform.localScale = new Vector3(-1, 1, 1);
			}
		});

		//tw.OnComplete(() =>
		//{
		//	_body.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
		//	_rtImage.transform.localScale = new Vector3(1, 1, 1);
		//	onComplete?.Invoke();
		//});
	}

}
