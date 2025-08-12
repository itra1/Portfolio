using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScrollingMenu : PanelUi {

	public RectTransform titlePanel;                               // Панель с заголовками разделов
	public RectTransform pagesPanel;                               // ПАнель со страницами
	public List<Pages> pageList;

	[SerializeField]
	protected RectTransform contentPanel;
	private float proportionX;
	private float pageFillX;
	private float speedMuve;
	private bool isMove;
	private int newPage;                                                // Страница куда хотим перейти
	private bool move;                                                  // Флаг что выполняется перемещение
	private int thisPage;                                               // Текущая страница
	private bool goToPage = false;

	public Color titileActive;                                  // Цвет активного заголовка
	public Color titleDisactive;                                // Цвет не активного заголовка

	[System.Serializable]
	public struct Pages {

		public Type type;
		public RectTransform title;
		public RectTransform page;

		private Text _titleText;
		public Text titleText {
			get {
				if (_titleText == null)
					_titleText = title.GetComponentInChildren<Text>();
				return _titleText;
			}
		}

		public void SetAllactive(bool isActive) {
			if (title != null)
				title.gameObject.SetActive(isActive);
			if (page != null)
				page.gameObject.SetActive(isActive);
		}

		public enum Type {
			quest,
			friend,
			result
		}

	}

	protected override void OnEnable() {
		base.OnEnable();

		if (GameManager.activeLevelData.gameMode == GameMode.survival) {
			pageList.Find(x => x.type == Pages.Type.quest).SetAllactive(false);
			pageList.RemoveAll(x => x.type == Pages.Type.quest);
		} else {
			//pageList.Find(x => x.type == Pages.Type.friend).page.gameObject.SetActive(false);
		}

		// Определяем размеры
		pageFillX = contentPanel.rect.width;
		speedMuve = pageFillX / 10;

		thisPage = 0;
		newPage = thisPage;

		// Смещаем каждую страничку правее
		for (int i = 0; i < pageList.Count; i++) {
			pageList[i].page.gameObject.SetActive(true);
			pageList[i].page.anchoredPosition = new Vector2(pageFillX * i, pageList[i].page.transform.localPosition.y);
		}

		titlePanel.anchoredPosition = new Vector2(-pageList[thisPage].title.anchoredPosition.x, titlePanel.anchoredPosition.y);

		pagesPanel.anchoredPosition = new Vector2(0 + pageFillX * -thisPage, pagesPanel.anchoredPosition.y);

		if (pageList.Count > 1)
			proportionX = pageFillX / pageList[1].title.anchoredPosition.x;
		else
			proportionX = 0;
		ColorUpdate();
	}

	void Update() {
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {

			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			if (Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y) && Mathf.Abs(touchDeltaPosition.x) > 1f)
				AllMove(touchDeltaPosition.x);

			isMove = true;
		}

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && !goToPage) {
			move = true;

			float diffX = Mathf.Abs(pagesPanel.transform.localPosition.x) - pageList[thisPage].page.transform.localPosition.x;
			if (thisPage == 0 && pagesPanel.transform.localPosition.x > 0) diffX *= -1;
			if (diffX > 0) newPage = thisPage + 1;
			if (diffX < 0) newPage = thisPage - 1;
			if (newPage < 0) newPage = 0;
			if (newPage > pageList.Count - 1) newPage = pageList.Count - 1;

			if (newPage != thisPage) {
				float diffXNew = Mathf.Abs(pagesPanel.transform.localPosition.x) - pageList[newPage].page.transform.localPosition.x;
				if (newPage == 0 && pagesPanel.transform.localPosition.x > 0) diffXNew *= -1;

				if (Mathf.Abs(diffXNew) / 2 < Mathf.Abs(diffX))
					thisPage = newPage;
				else
					newPage = thisPage;
			}
		}

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
			if (isMove) isMove = false;

		if (move) MoveToPage();
	}

	void AllMove(float alldiff) {
		titlePanel.anchoredPosition = new Vector2(titlePanel.anchoredPosition.x + alldiff / proportionX, titlePanel.anchoredPosition.y);

		pagesPanel.anchoredPosition = new Vector2(pagesPanel.anchoredPosition.x + alldiff, pagesPanel.anchoredPosition.y);
		ColorUpdate();
	}

	void MoveToPage() {

		float diffX = Mathf.Abs(pagesPanel.transform.localPosition.x) - pageList[thisPage].page.transform.localPosition.x;
		if (thisPage == 0 && pagesPanel.transform.localPosition.x > 0) diffX *= -1;

		if (diffX != 0) {
			float moveX = 0;
			moveX = speedMuve * Mathf.Sign(diffX);

			if (Mathf.Abs(diffX) - Mathf.Abs(moveX) < 0) moveX = Mathf.Abs(diffX) * Mathf.Sign(diffX);

			AllMove(moveX);
		} else {
			move = false;
			goToPage = false;
		}
		ColorUpdate();
	}

	public void moveToPageButton(int goPage) {

		//if(!UiController.checkStart()) return;

		if (thisPage != goPage) {
			thisPage = goPage;
			goToPage = true;
			move = true;
		}
	}

	/// <summary>
	/// Изменение прозрачности заголовка в зависимости от удаления от центра
	/// </summary>
	void ColorUpdate() {
		for (int i = 0; i < pageList.Count; i++) {
			float diff = Mathf.Abs(Mathf.Abs(pagesPanel.transform.localPosition.x) - pageList[i].page.transform.localPosition.x);

			if (diff > pageFillX / 3)
				pageList[i].titleText.color = titleDisactive;
			else
				pageList[i].titleText.color = new Color(titileActive.r, titileActive.g, titileActive.b, ((titileActive.a - titleDisactive.a) / (pageFillX / 3) * ((pageFillX / 3) - diff)) + titleDisactive.a);
		}
	}
}
