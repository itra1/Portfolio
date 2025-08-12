using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

namespace it.UI.GameMenu
{
  public class GameMenu : UIDialog
  {

	 public UnityEngine.Events.UnityAction onContinueButtonClick;
	 public UnityEngine.Events.UnityAction onLoadLastClick;
	 public UnityEngine.Events.UnityAction onSettingsButtonClick;
	 public UnityEngine.Events.UnityAction onAboutButtonClick;
	 public UnityEngine.Events.UnityAction onExitButtonClick;


	 [SerializeField]
	 private FirtMenu.Button _continueButton;
	 [SerializeField]
	 private FirtMenu.Button _loadLastButton;
	 [SerializeField]
	 private FirtMenu.Button _settingsButton;
	 [SerializeField]
	 private FirtMenu.Button _abourButton;
	 [SerializeField]
	 private FirtMenu.Button _exitButton;

	 private FirtMenu.Button _selectButton;

	 [SerializeField]
	 private RectTransform _roundIcone;

	 [SerializeField]
	 private Transform _selectImage;

	 private List<MenuColorData> imagesMenuList = new List<MenuColorData>();

	 private struct MenuColorData
	 {
		public Text text;
		public Image image;
		public Color targetColor;
		public Color transparentColor;
	 }
	 private void Start()
	 {

		if (imagesMenuList.Count == 0)
		  FindImagesMenu();
	 }

	 private void FindImagesMenu()
	 {
		imagesMenuList.Clear();

		Image[] menuImages = this.GetComponentsInChildren<Image>(true);

		for (int i = 0; i < menuImages.Length; i++)
		{
		  Color c = menuImages[i].color;
		  c.a = 0;
		  imagesMenuList.Add(new MenuColorData()
		  {
			 image = menuImages[i],
			 targetColor = menuImages[i].color,
			 transparentColor = c
		  });
		}
		Text[] menuTexts = this.GetComponentsInChildren<Text>();

		for (int i = 0; i < menuTexts.Length; i++)
		{
		  Color c = menuTexts[i].color;
		  c.a = 0;
		  imagesMenuList.Add(new MenuColorData()
		  {
			 text = menuTexts[i],
			 targetColor = menuTexts[i].color,
			 transparentColor = c
		  });
		}

	 }

	 public override void Escape()
	 {
		if (_isEscapeProcess)
		  return;
		base.Escape();

		ShowMenuAnimated(false, 1,()=>{
		  Hide(0);
		});


	 }


	 protected override void OnEnable()
	 {
		if (imagesMenuList.Count == 0)
		  FindImagesMenu();

		base.OnEnable();
		SubscribeButtons();
		OnFocus(_continueButton);
		ShowMenuAnimated(true,1);
	 }

	 private void Update()
	 {
		//Крутим кольцо
		_roundIcone.rotation *= Quaternion.Euler(0, 0, 3 * Time.deltaTime);
	 }
	 public void ShowMenuAnimated(bool isVisible, float time = 2, System.Action onComplete = null)
	 {

		foreach (MenuColorData mcd in imagesMenuList)
		{
		  Color source = isVisible ? mcd.transparentColor : mcd.targetColor;
		  Color target = isVisible ? mcd.targetColor : mcd.transparentColor;


		  if (mcd.image != null)
		  {
			 mcd.image.color = source;
			 mcd.image.DOColor(target, time);
		  }
		  else if(mcd.text != null)
		  {
			 mcd.text.color = source;
			 mcd.text.DOColor(target, time);
		  }
		}
		DOVirtual.DelayedCall(time, ()=> { onComplete?.Invoke(); });

	 }

	 private void SubscribeButtons()
	 {
		_loadLastButton.onClick = LoadLastButton;
		_continueButton.onClick = ContinueButton;
		_settingsButton.onClick = SettingsButton;
		_abourButton.onClick = AboutButton;
		_exitButton.onClick = ExitButton;

		_loadLastButton.onFocus = OnFocus;
		_continueButton.onFocus = OnFocus;
		_settingsButton.onFocus = OnFocus;
		_abourButton.onFocus = OnFocus;
		_exitButton.onFocus = OnFocus;
	 }

	 private void OnFocus(it.UI.FirtMenu.Button btn)
	 {
		_selectButton = btn;
		_loadLastButton.SetSelect(_selectButton);
		_continueButton.SetSelect(_selectButton);
		_settingsButton.SetSelect(_selectButton);
		_abourButton.SetSelect(_selectButton);
		_exitButton.SetSelect(_selectButton);

		_selectImage.SetParent(_selectButton.transform);
		_selectImage.transform.localScale = Vector3.one;
		_selectImage.transform.localPosition = Vector3.zero;
	 }

	 /// <summary>
	 /// Запуск игры
	 /// </summary>
	 public void LoadLastButton()
	 {
		onLoadLastClick?.Invoke();
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

  }
}