using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftElement : MonoBehaviour {

	public Image icon;
	public Text countText;
	public List<GiftElementParam> pars;

	public System.Action MoveComplete;

	public Animation animPomponent;

	public Transform targetPosition;
	public RectTransform graphicTransform;

	private bool _isMove;

	private Vector3 startPosition;

	public bool noMove;

	private bool _readyTouch;

	private GiftElement.Type type;

	public bool toWindet;

	private GiftElementParam gift;

	private ICoinsPanelParent coinsPanel;

	private IMiniMenuParent miniMenuPanel;

	private void OnEnable() {
		_isMove = false;
		startPosition = transform.position;
		animPomponent.Play("firstShow");
		AudioManager.Instance.library.PlayGiftShowAudio();
	}

	[System.Serializable]
	public class GiftElementParam {
		public GiftElement.Type type;
		public Sprite sprite;
		public int count;
	}

	public void SetRandom(ICoinsPanelParent coinsPanel, IMiniMenuParent miniMenuPanel) {
		GiftElementParam elem = pars[Random.Range(0, pars.Count)];
		Show(elem, elem.count, coinsPanel, miniMenuPanel);
	}

	public void Show(Type type, ICoinsPanelParent coinsPanel, IMiniMenuParent miniMenuPanel) {
		GiftElementParam elem = pars.Find(x => x.type == type);
		Show(elem, elem.count, coinsPanel, miniMenuPanel);
	}

	public void Show(Type type, int count, ICoinsPanelParent coinsPanel, IMiniMenuParent miniMenuPanel) {
		GiftElementParam elem = pars.Find(x => x.type == type);
		Show(elem, count, coinsPanel, miniMenuPanel);

	}

	public void Show(GiftElementParam gift, int count, ICoinsPanelParent coinsPanel, IMiniMenuParent miniMenuPanel) {

		this.coinsPanel = coinsPanel;
		this.miniMenuPanel = miniMenuPanel;

		type = gift.type;

		this.gift = gift;
		FindTargetWindet();

		icon.sprite = gift.sprite;
		switch (gift.type) {
			case Type.coins15:
			case Type.coins30:
			case Type.coins50:
				icon.rectTransform.sizeDelta = new Vector2(80, 80 / gift.sprite.rect.width * gift.sprite.rect.height);
				break;
			default:
				icon.rectTransform.sizeDelta = new Vector2(200, 200 / gift.sprite.rect.width * gift.sprite.rect.height);
				break;
		}

		countText.text = count.ToString();
		_isMove = true;
		gameObject.SetActive(false);
	}

	public void FindTargetWindet() {

		if (!toWindet) return;
		_isMove = true;

		//PlayGamePlay gp = (PlayGamePlay)UIManager.Instance.GetPanel(UiType.game);

		if (gift.type == Type.coins15 || gift.type == Type.coins30 || gift.type == Type.coins50) {
			this.coinsPanel.CoinsPanelGiftOpen();
			targetPosition = this.coinsPanel.coinsTargetIcon.transform;
		} else {
			this.miniMenuPanel.MiniMenyGiftOpen(true);
			switch (gift.type) {
				case Type.hintAnyletter:
					targetPosition = this.miniMenuPanel.miniMenu.anyLetterBtnGraph.transform;
					break;
				case Type.hintLetter:
					targetPosition = this.miniMenuPanel.miniMenu.firstLetterBtnGraph.transform;
					break;
				case Type.hintWord:
					targetPosition = this.miniMenuPanel.miniMenu.anyWordBtnGraph.transform;
					break;
			}
		}

	}

	private Vector3 _startScale;

	private void Update() {

		if (!_isMove || noMove) return;

		Vector3 newPosition = transform.position + (targetPosition.position - transform.position).normalized * 10 * Time.deltaTime;
		if ((targetPosition.position - transform.position).magnitude >
				(newPosition - transform.position).magnitude) {
			transform.position = newPosition;

			graphicTransform.localScale = Vector3.Lerp(_startScale, Vector3.one,
				1 - ((targetPosition.position - transform.position).magnitude / (targetPosition.position - startPosition).magnitude));

			return;
		}

		graphicTransform.localScale = Vector3.one;

		transform.position = targetPosition.position;
		_isMove = false;

		if (toWindet) {
			//PlayGamePlay gp = (PlayGamePlay) UIManager.Instance.GetPanel(UiType.game);

			switch (gift.type) {
				case Type.coins15:
				case Type.coins30:
				case Type.coins50:
					this.coinsPanel.coinsWidget.PlayerBauns();
					break;
				case Type.hintAnyletter:
					this.miniMenuPanel.miniMenu.anyLetterBtn.GetComponent<HintMenuButton>().PlayBauns();
					break;
				case Type.hintLetter:
					this.miniMenuPanel.miniMenu.firstLetterBtn.GetComponent<HintMenuButton>().PlayBauns();
					break;
				case Type.hintWord:
					this.miniMenuPanel.miniMenu.anyWordBtn.GetComponent<HintMenuButton>().PlayBauns();
					break;
			}
		}

		MoveComplete();
		this.miniMenuPanel.MiniMenyGiftClose(true);
		if (toWindet)
			gameObject.SetActive(false);
	}

	public void FirstShowComplete() {
		_isMove = true;
		_startScale = graphicTransform.localScale;
	}

	public enum Type {
		coins15,
		coins30,
		coins50,
		hintAnyletter,
		hintLetter,
		hintWord
	}

}
