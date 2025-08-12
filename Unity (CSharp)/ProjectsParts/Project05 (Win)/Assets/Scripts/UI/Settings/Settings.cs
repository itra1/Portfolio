using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using I2.Loc;

namespace it.UI.Settings {
  public class Settings : UIDialog
  {
    public UnityEngine.Events.UnityAction onOkButtonClick;
    public UnityEngine.Events.UnityAction onCancelButtonClick;
    public UnityEngine.Events.UnityAction<int> onFpsChange;
    public UnityEngine.Events.UnityAction<int> onQualityLevelChange;

    [SerializeField] private RectTransform _selectLine;
    [SerializeField] private GameObject _dialog;
    [SerializeField] private Color _selectColor;
    [SerializeField] private Dropdown _qualitySettings = null;
    [SerializeField] private Toggle[] _fps;

    private PageButton[] _pageButtons;
    private PageBase[] _pages;
    private PageType _page;
    private List<MenuColorData> _imagesMenuList = new List<MenuColorData>();
    private struct MenuColorData
    {
      public TextMeshProUGUI Text;
      public Image Image;
      public Color TargetColor;
      public Color TransparentColor;
    }
    private void FindImagesMenu()
    {
      _imagesMenuList.Clear();

      Image[] menuImages = _dialog.GetComponentsInChildren<Image>(true);

      for (int i = 0; i < menuImages.Length; i++)
      {
        Color c = menuImages[i].color;
        c.a = 0;
        _imagesMenuList.Add(new MenuColorData()
        {
          Image = menuImages[i],
          TargetColor = menuImages[i].color,
          TransparentColor = c
        });
      }


      TextMeshProUGUI[] menuTexts = _dialog.GetComponentsInChildren<TextMeshProUGUI>();

      for (int i = 0; i < menuTexts.Length; i++)
      {
        Color c = menuTexts[i].color;
        c.a = 0;
        _imagesMenuList.Add(new MenuColorData()
        {
          Text = menuTexts[i],
          TargetColor = menuTexts[i].color,
          TransparentColor = c
        });
      }

    }

	 private void Start()
	 {
      FindAll();
      DeactivaAllPage();
      SetPage(PageType.game, true);

      if (_imagesMenuList.Count <= 0)
        FindImagesMenu();
    }

    private void FindAll()
	 {
      _pageButtons = GetComponentsInChildren<PageButton>(true);
      _pages = GetComponentsInChildren<PageBase>(true);
    }

	 protected override void OnEnable()
    {
      FindAll();
      DeactivaAllPage();
      SetPage(PageType.game, true);
      if(_imagesMenuList.Count <= 0)
        FindImagesMenu();

      foreach (var elem in _pageButtons)
        elem.OnClickEvent += ClickPageButton;

      //_qualitySettings.onValueChanged.AddListener(QualityChange);
      //_qualitySettings.SetValueWithoutNotify(it.Game.Managers.GameManager.Instance.GameSettings.QualityLevel);

      //_fps[0].SetIsOnWithoutNotify(it.Game.Managers.GameManager.Instance.GameSettings.TargetFps == -1);
      //_fps[1].SetIsOnWithoutNotify(it.Game.Managers.GameManager.Instance.GameSettings.TargetFps == 60);
      //_fps[2].SetIsOnWithoutNotify(it.Game.Managers.GameManager.Instance.GameSettings.TargetFps == 30);


      base.OnEnable();
    }

	 public override void Show()
	 {
		base.Show();
      ShowMenuAnimated(1);

    }

	 public override void Hide(float timeHide = 1f)
	 {
		base.Hide(timeHide);
      HideMenuAnimated(1);
    }


	 public void ShowMenuAnimated(float time = 2, System.Action onComplete = null)
    {
      _dialog.SetActive(true);
      foreach (MenuColorData mcd in _imagesMenuList)
      {
        if (mcd.Image != null)
        {
          mcd.Image.color = mcd.TransparentColor;
          mcd.Image.DOColor(mcd.TargetColor, time);
        }
        if(mcd.Text != null)
        {
          mcd.Text.color = mcd.TransparentColor;
          mcd.Text.DOColor(mcd.TargetColor, time);
        }
      }

      DOVirtual.DelayedCall(time, () =>
      {
        onComplete?.Invoke();
      });

    }

    public void HideMenuAnimated(float time = 2, System.Action onComplete = null)
    {
      _dialog.SetActive(true);
      foreach (MenuColorData mcd in _imagesMenuList)
      {
        if (mcd.Image != null)
          mcd.Image.DOColor(mcd.TransparentColor, time);
        if (mcd.Text != null)
          mcd.Text.DOColor(mcd.TransparentColor, time);
      }

        DOVirtual.DelayedCall(time, () =>
      {
        onComplete?.Invoke();
      });
    }

    public void BackButton() {
      Hide(1.5f);
    }

    protected override void OnDisable() {
      //_qualitySettings.onValueChanged.RemoveAllListeners();
      base.OnDisable();
    }

    private PageButton GetPageButton(PageType pageType) {

      for (int i = 0; i < _pageButtons.Length; i++)
        if ((_pageButtons[i].Page & pageType) != 0)
          return _pageButtons[i];
      return _pageButtons[0];
    }
    private PageButton GetFocusPageButton(PageType pageType) {

      for (int i = 0; i < _pageButtons.Length; i++)
        if ((_pageButtons[i].FocusPage & pageType) != 0)
          return _pageButtons[i];
      return _pageButtons[0];
    }

    private PageBase GetPage(PageType pageType) {

      for (int i = 0; i < _pages.Length; i++)
        if ((_pages[i].Page & pageType) != 0)
          return _pages[i];
      return _pages[0];
    }

    public void SetPage(PageType pageType, bool force = false) {
      ChangePageButton(pageType, force);
      ChangePage(pageType, force);
      _page = pageType;

    }
    private void DeactivaAllPage() {
      for (int i = 0; i < _pages.Length; i++)
        _pages[i].gameObject.SetActive(false);
    }

    public void ClickPageButton(PageButton page) {
      SetPage(page.Page);
    }

    private void ChangePage(PageType pageType, bool force) {
      ChangePage(GetPage(pageType), force);
    }
    private void ChangePageButton(PageType pageType, bool force) {
      ChangePageButton(GetFocusPageButton(pageType), force);
    }
    private void ChangePage(PageBase page, bool force) {
      PageBase currentPage = GetPage(_page);

      if (!force && currentPage.Page == page.Page)
        return;

      currentPage.gameObject.SetActive(false);
      page.gameObject.SetActive(true);
    }

    private void ChangePageButton(PageButton page, bool force) {
      PageButton currentPageButton = GetPageButton(_page);
      if (!force && currentPageButton.Page == page.Page)
        return;

      TextMeshProUGUI curText = currentPageButton.GetComponentInChildren<TextMeshProUGUI>();
      RectTransform buttonRect = page.GetComponent<RectTransform>();
      TextMeshProUGUI textPro = page.GetComponentInChildren<TextMeshProUGUI>();
      Vector2 tRect = _selectLine.sizeDelta;

      if (force) {
        curText.color = Color.white;
        _selectLine.anchoredPosition = new Vector2(page.GetComponent<RectTransform>().anchoredPosition.x, _selectLine.anchoredPosition.y);
        _selectLine.sizeDelta = new Vector2(textPro.preferredWidth, _selectLine.sizeDelta.y);
        textPro.color = _selectColor;
      } else {
        DOTween.To(() => curText.color, (x) => curText.color = x, Color.white, 0.3f);
        DOTween.To(() => _selectLine.anchoredPosition, (x) => _selectLine.anchoredPosition = x, new Vector2(page.GetComponent<RectTransform>().anchoredPosition.x, _selectLine.anchoredPosition.y), 0.3f);
        DOTween.To(() => _selectLine.sizeDelta, (x) => _selectLine.sizeDelta = x, new Vector2(textPro.preferredWidth, _selectLine.sizeDelta.y), 0.3f);
        DOTween.To(() => textPro.color, (x) => textPro.color = x, _selectColor, 0.3f);
      }
    }

    public void QualityChange(int value) {
      onQualityLevelChange?.Invoke(value);
    }
    public void FpsChange(int value) {
      onFpsChange?.Invoke(value);
    }

    public void OkButton() {
      onOkButtonClick?.Invoke();
    }

    public void CancelButton() {
      onCancelButtonClick?.Invoke();
    }
    [System.Flags]
    public enum PageType {
      none = 0,
      game = 1, 
      video = 2, 
      audio = 4, 
      control = 8, 
      keyboard = 16, 
      gamepad = 32
    }

    public override void Escape() {
      base.Escape();
      BackButton();
    }

    //private void Update() {
    //  Input.
    //}

  }
}