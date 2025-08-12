using UnityEngine;
using System.Collections.Generic;
using DarkTonic;
using it.UI.GameMenu;
using it.Game.Managers;
using UnityEngine.UI;
using DG.Tweening;

namespace it.UI.FirtMenu
{
  /// <summary>
  /// Первоначальное меню
  /// </summary>
  public class FirstMenu : UIDialog
  {
	 public static bool IS_LAST_SHOW = false;
	 public UnityEngine.Events.UnityAction onGameButtonClick;
	 public UnityEngine.Events.UnityAction onContinueButtonClick;
	 public UnityEngine.Events.UnityAction onSettingsButtonClick;
	 public UnityEngine.Events.UnityAction onAboutButtonClick;
	 public UnityEngine.Events.UnityAction onExitButtonClick;
	 public UnityEngine.Events.UnityAction<int> onLoadSceneClick;
	 public UnityEngine.Events.UnityAction onPlayBackAudio;

	 [SerializeField]
	 private Image _miniLogo;
	 [SerializeField]
	 private Image _bigLogo;

	 [SerializeField]
	 private GameObject _menu;
	 [SerializeField]
	 private RectTransform _roundIcone;

	 [SerializeField]
	 private GameObject _version;

	 [SerializeField]
	 private Button _newGameButton;
	 [SerializeField]
	 private Button _continueButton;
	 [SerializeField]
	 private Button _settingsButton;
	 [SerializeField]
	 private Button _abourButton;
	 [SerializeField]
	 private Button _exitButton;

	 private Button _selectButton;

	 [SerializeField]
	 private Transform _selectImage;

	 private List<MenuColorData> imagesMenuList = new List<MenuColorData>();

	 private struct MenuColorData
	 {
		public Image image;
		public Color targetColor;
		public Color transparentColor;
	 }

	 protected override void OnEnable()
	 {
		base.OnEnable();
		SubscribeButtons();
		FocusItem(_newGameButton, false);
		_version.SetActive(false);
		FindImagesMenu();

		if (!IS_LAST_SHOW)
		{
		  AnimateVisible();

		  IS_LAST_SHOW = true;
		  return;
		}

		_bigLogo.gameObject.SetActive(false);

	 }

	 private void FindImagesMenu()
	 {
		imagesMenuList.Clear();

		Image[] menuImages = _menu.GetComponentsInChildren<Image>();

		for(int i = 0; i < menuImages.Length; i++)
		{
		  Color c = menuImages[i].color;
		  c.a = 0;
		  imagesMenuList.Add(new MenuColorData()
		  {
			 image = menuImages[i],
			 targetColor = menuImages[i].color,
			 transparentColor = c
		  }) ;
		}
	 }

	 private void AnimateVisible()
	 {
		DOVirtual.DelayedCall(1.5f, () => {
		  DarkTonic.MasterAudio.MasterAudio.PlaySound("UI_ALL", 1, null, 0, "Menu_whoosh");
		});

		_bigLogo.color = new Color(0, 0, 0, 0);
		_miniLogo.gameObject.SetActive(false);
		_menu.SetActive(false);
		_roundIcone.gameObject.SetActive(false);
		_bigLogo.DOColor(new Color(1, 1, 1, 1), 3f).OnComplete(() => {

		  DOVirtual.DelayedCall(2, ShowCircle);

		  DOVirtual.DelayedCall(4, () => {
			 _bigLogo.DOColor(new Color(1, 1, 1, 0), 3).OnComplete(() =>
			 {
				ShowMenuAnimated();
			 });
		  });

		});
	 }

	 private void ShowCircle()
	 {
		Image cir = _roundIcone.GetComponent<Image>();
		Color selfColor = cir.color;
		Color useColor = selfColor;
		useColor.a = 0;
		cir.color = useColor;
		_roundIcone.gameObject.SetActive(true);
		cir.DOColor(selfColor, 2);
	 }

	 public void ShowMenuAnimated(float time = 2, System.Action onComplete = null)
	 {

	 onPlayBackAudio?.Invoke();


	 _menu.SetActive(true);
		foreach (MenuColorData mcd in imagesMenuList)
		{
		  mcd.image.color = mcd.transparentColor;
		  mcd.image.DOColor(mcd.targetColor, time);
		}

		_miniLogo.gameObject.SetActive(true);
		Color logoColor = _miniLogo.color;
		Color trans = logoColor;
		trans.a = 0;
		_miniLogo.color = trans;
		_miniLogo.DOColor(logoColor, time).OnComplete(()=> { onComplete?.Invoke(); });
	 }

	 public void HideMenuAnimated(float time = 2, System.Action onComplete = null)
	 {
		_menu.SetActive(true);
		foreach (MenuColorData mcd in imagesMenuList)
		{
		  mcd.image.DOColor(mcd.transparentColor, time);
		}

		_miniLogo.gameObject.SetActive(true);
		Color logoColor = _miniLogo.color;
		Color trans = logoColor;
		trans.a = 0;
		logoColor.a = 1;
		_miniLogo.DOColor(trans, time).OnComplete(() => { onComplete?.Invoke(); });
	 }


	 private void Update()
	 {
		//Крутим кольцо
		_roundIcone.rotation *= Quaternion.Euler(0, 0, 3 * Time.deltaTime);
	 }


	 private void SubscribeButtons()
	 {
		_newGameButton.onClick = GameButton;
		_continueButton.onClick = ContinueButton;
		_settingsButton.onClick = SettingsButton;
		_abourButton.onClick = AboutButton;
		_exitButton.onClick = ExitButton;
		_newGameButton.onFocus = OnFocus;
		_continueButton.onFocus = OnFocus;
		_settingsButton.onFocus = OnFocus;
		_abourButton.onFocus = OnFocus;
		_exitButton.onFocus = OnFocus;
	 }

	 private void FocusItem(Button btn, bool playSound = true)
	 {
		if(playSound)
		DarkTonic.MasterAudio.MasterAudio.PlaySound("UI_Item_Select");

		_selectButton = btn;
		_newGameButton.SetSelect(_selectButton);
		_continueButton.SetSelect(_selectButton);
		_settingsButton.SetSelect(_selectButton);
		_abourButton.SetSelect(_selectButton);
		_exitButton.SetSelect(_selectButton);

		_selectImage.SetParent(_selectButton.transform);
		_selectImage.transform.localScale = Vector3.one;
		_selectImage.transform.localPosition = Vector3.zero;
	 }

	 private void OnFocus(Button btn)
	 {
		FocusItem(btn);
	 }

	 /// <summary>
	 /// Запуск игры
	 /// </summary>
	 public void GameButton()
	 {
		onGameButtonClick?.Invoke();
	 }
	 /// <summary>
	 /// Продолжение
	 /// </summary>
	 private void ContinueButton()
	 {
		onContinueButtonClick?.Invoke();
	 }
	 /// <summary>
	 /// Настройки
	 /// </summary>
	 private void SettingsButton()
	 {
		onSettingsButtonClick?.Invoke();
	 }
	 /// <summary>
	 /// Абоут
	 /// </summary>
	 private void AboutButton()
	 {
		onAboutButtonClick?.Invoke();
	 }

	 /// <summary>
	 /// Кнопка выхода
	 /// </summary>
	 public void ExitButton()
	 {
		onExitButtonClick?.Invoke();
	 }

	 public void StartLevel(int level)
	 {
		onLoadSceneClick?.Invoke(level);
	 }
  }
}