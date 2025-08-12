using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LanguageUi : UiPanel {

	private List<LanuageLibrary> _translateLanguageList = new List<LanuageLibrary>();
	private LanuageLibrary _activeTranslateLang;
	private int _activePageTranslate;

	public LanguageUiElement prefabElement;
	private List<LanguageUiElement> elementsList = new List<LanguageUiElement>();

	public Animation animComp;

	public Transform content;

	private LanuageLibrary _selectedElement;

	private bool _isMainLanguage;

	public GameObject mainTitleBlock;
	public GameObject secondTitleBlock;

	public bool isMainLanguage {
		set {
			_isMainLanguage = value;
			mainTitleBlock.SetActive(_isMainLanguage);
			secondTitleBlock.SetActive(!_isMainLanguage);
		}
	}

	protected override void OnEnable() {
		base.OnEnable();
	}

	public void View(bool isMainLang) {
		isMainLanguage = isMainLang;

		if (_isMainLanguage) {
			InitMainLanguage();
		} else {
			InitTranslateLanuage();
		}
		SpawnElements();
		CheckSelected();
		//InitTranslateLanuage();
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		AudioManager.Instance.library.PlayWindowOpenAudio();
		animComp.Play("show");
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		AudioManager.Instance.library.PlayWindowCloseAudio();
		animComp.Play("hide");
	}

	public void CloseButton() {
		//AudioManager.Instance.library.PlayClickAudio();
		if (isClosing) return;
		isClosing = true;
		Hide(() => {
			isClosing = false;
			gameObject.SetActive(false);
		});
	}

	private void InitTranslateLanuage() {
		_translateLanguageList.Clear();

		GameCompany.Word word = PlayerManager.Instance.company.GetActualCompany().locations[0].levels[0].words[0];

		_translateLanguageList.Add(LanguageManager.Instance.lanuageLibrary.Find(x => x.code == "none"));

		word.translations.ForEach((elem) => {
			LanuageLibrary ll = LanguageManager.Instance.lanuageLibrary.Find(x => x.code == elem.lang && x.selectTranslateList);
			if (ll != null) {
				_translateLanguageList.Add(ll);
			}
		});
		_activePageTranslate = 0;
		for (int i = 0; i < _translateLanguageList.Count; i++) {
			if (_translateLanguageList[i].code == PlayerManager.Instance.translateLanuage)
				_activePageTranslate = i;
		}
	}

	private void InitMainLanguage() {
		_translateLanguageList.Clear();

		GameCompany.Word word = PlayerManager.Instance.company.companies[0].locations[0].levels[0].words[0];

		_translateLanguageList = new List<LanuageLibrary>(LanguageManager.Instance.lanuageLibrary.FindAll(x => x.isGameLanguage));

		_activePageTranslate = 0;
		for (int i = 0; i < _translateLanguageList.Count; i++) {
			if (_translateLanguageList[i].code == PlayerManager.Instance.company.actualCompany)
				_activePageTranslate = i;
		}
	}

	private void CheckSelected() {

		if (_isMainLanguage) {
			CheckCompanySelect();
		} else {
			CheckTranslateSelect();
		}
	}

	private void CheckTranslateSelect() {

		var Element = _translateLanguageList.Find(x => x.code == PlayerManager.Instance.translateLanuage);

		if (Element == null) {
			Element = new LanuageLibrary();
			Element.code = "none";
		}

		elementsList.ForEach(x => x.SetSelected(Element));
	}

	private void CheckCompanySelect() {

		var Element = _translateLanguageList.Find(x => x.code == PlayerManager.Instance.company.actualCompany);

		if (Element == null) {
			Element = new LanuageLibrary();
			Element.code = "en";
		}
		elementsList.ForEach(x => x.SetSelected(Element));
	}

	private void SpawnElements() {

		if (elementsList.Count == _translateLanguageList.Count) return;

		prefabElement.gameObject.SetActive(false);

		elementsList.ForEach(x => Destroy(x.gameObject));
		int menu = 0;
		_translateLanguageList.ForEach(x => {

			GameObject el = Instantiate(prefabElement.gameObject);
			el.transform.SetParent(content);
			el.transform.localScale = Vector3.one;
			el.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -26 - menu * 135);
			el.GetComponent<RectTransform>().sizeDelta = prefabElement.GetComponent<RectTransform>().sizeDelta;
			el.gameObject.SetActive(true);
			LanguageUiElement uel = el.GetComponent<LanguageUiElement>();
			uel.SetData(x);
			uel.OnDown = ElementDown;
			uel.OnUp = ElementUp;
			elementsList.Add(uel);
			menu++;
		});

	}

	private bool isPoint;
	private Vector3 mousePoint = Vector3.zero;
	private Vector3 deltaMove = Vector3.zero;
	private void Update() {

		if (!isPoint && Input.GetMouseButtonDown(0)) {
			mousePoint = Input.mousePosition;
		}

		if (isPoint) {
			deltaMove = Input.mousePosition - mousePoint;
			if (deltaMove.y > 0)
				isDrag = true;
			Debug.Log(deltaMove);
			mousePoint = Input.mousePosition;
		}

		if (isPoint && Input.GetMouseButtonUp(0)) {

		}

	}

	private void ElementDown(LanuageLibrary elem) {
		//PlayerManager.Instance.translateLanuage = elem.code;
		//CheckSelected();
		_selectedElement = elem;

		if (_isMainLanguage)
			PlayerManager.Instance.company.actualCompany = _selectedElement.code;
		else
			PlayerManager.Instance.translateLanuage = _selectedElement.code;
	}

	private void ElementUp(LanuageLibrary elem) {
		if (!isDrag) {
			// Обработка выбора
		};
		isDrag = false;

		CloseButton();
	}

	private bool isDrag = false;

	public void ViewDown(PointerEventData eventData) {
		isDrag = false;
	}

	public void ViewUp(PointerEventData eventData) {
		isDrag = false;
	}

	public void ViewDrag(PointerEventData eventData) {
		isDrag = false;
	}

	public override void ManagerClose() {
		CloseButton();
	}

}
