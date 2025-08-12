/*
  Контроллер управления главным экраном
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
#if PLUGIN_FACEBOOK
using Facebook.Unity;
#endif



public class MainPanelController : MonoBehaviour {

	public GameManager manager;
	public GameObject mainCamera;                   // Ссылка на объект камеры
	public GameObject bottomPanel;                  // Панель основной навигации
	public GameObject bestPanel;                    // Панель лучшего результата

	public GameObject settingsPanel;                // Панель настроек

	public Image settingsButIcon;                           // Иконка кнопки настроек
	public Text settingsButText;                            // Текст кнопки настроек

	public Text counsCountMain;                             // Текст количество элементов
	public Text cristallCountMain;                          // Количество кристалов
	public Text blackMarckCountMain;                        // Текст с количеством черныйх меток
	public Text maxDistantionMain;                          // Текст максимальной дистанции

	Animator animComp;
	bool fbAuth;

	#region Colors
	public Color whiteColor;
	public Color yellowColor;
	#endregion

	#region Settings

	public Image musicIcon;
	public Image effectIcon;


	public Sprite musicOffIcon;
	public Sprite musicOnIcon;
	public Sprite effectOnIcon;
	public Sprite effectOffIcon;

	[HideInInspector]
	public bool waitHide;          // Ожидание закрытия
	#endregion

	public AudioClip clickClip;

	void OnDisable() {
		bottomPanel.SetActive(false);
		bestPanel.SetActive(false);
		fbAuthPanel.SetActive(false);
		fbProfilPanel.SetActive(false);
		settingsPanel.SetActive(false);
	}

	void Start() {
		bottomPanel.SetActive(false);
		bestPanel.SetActive(false);
		fbAuthPanel.SetActive(false);
		fbProfilPanel.SetActive(false);
		settingsPanel.SetActive(false);
		animComp = GetComponent<Animator>();
	}

	void OnEnable() {
		fbAuth = false;
		bottomPanel.SetActive(false);
		bestPanel.SetActive(false);
		fbAuthPanel.SetActive(false);
		fbProfilPanel.SetActive(false);
		settingsPanel.SetActive(false);
		animComp = GetComponent<Animator>();

		ShowCounts();

		FacebookAuth();

		#region Settings
		int music = PlayerPrefs.GetInt("audioMusic");
		int effects = PlayerPrefs.GetInt("audioEffects");

		if (music == 1)
			musicIcon.sprite = musicOnIcon;
		else
			musicIcon.sprite = musicOffIcon;


		if (effects == 1)
			effectIcon.sprite = effectOnIcon;
		else
			effectIcon.sprite = effectOffIcon;
		#endregion
	}

	void Update() {
		if (!fbAuth) {
			FacebookAuth();
		}
	}

	/* *****************************
	 * Отображение значений количеств
	 * *****************************/
	public void ShowCounts() {
		// Монеты
		int coins = UserManager.coins;
		counsCountMain.text = coins.ToString();

		// Кристаллы
		int cristall = UserManager.Instance.cristall;
		cristallCountMain.text = cristall.ToString();

		// Черный метки
		int blackMark = UserManager.Instance.blackMark;
		blackMarckCountMain.text = blackMark.ToString();

		//Максимальная дистанция
		int maxDistance = (int)UserManager.Instance.survivleMaxRunDistance;
		maxDistantionMain.text = maxDistance.ToString();
	}

	#region Settings
	public void ChangeMusic() {
		int music = PlayerPrefs.GetInt("audioMusic");

		if (music == 1) {
			music = 0;
			musicIcon.sprite = musicOffIcon;
		} else {
			music = 1;
			musicIcon.sprite = musicOnIcon;
		}

		PlayerPrefs.SetInt("audioMusic", music);
		PlayerPrefs.Save();

		//GameManager.ChangeValue();
		AudioManager.Instance.SetSoundParametrs();
	}


	public void ChangeEffect() {
		int effects = PlayerPrefs.GetInt("audioEffects");


		if (effects == 1) {
			effects = 0;
			effectIcon.sprite = effectOffIcon;
		} else {
			effects = 1;
			effectIcon.sprite = effectOnIcon;
		}
		PlayerPrefs.SetInt("audioEffects", effects);
		PlayerPrefs.Save();

		//GameManager.ChangeValue();
		AudioManager.Instance.SetSoundParametrs();
	}


	public void HideThis() {
		if (waitHide) return;

		waitHide = true;
		animComp.SetTrigger("hide");
		StartCoroutine(waitAndDisable());
	}

	public void SettingsShow() {
		AudioManager.PlayEffect(clickClip, AudioMixerTypes.runnerEffect);

		if (settingsPanel.activeInHierarchy)
			animComp.SetTrigger("settingOff");
		else
			animComp.SetTrigger("settingOn");
	}

	#endregion

	#region Facebook

	public GameObject fbAuthPanel;                  // Панель авторизации в ФБ
	public GameObject fbProfilPanel;                // Панель профиля ФБ

	// Событие Нажатия кнопки
	public void FBLogin() {
#if PLUGIN_FACEBOOK
		AudioManager.PlayEffects(clickClip, AudioMixerTypes.runnerEffect);
		if (FBController.instance != null) {
			FBController.instance.FBlogin(FacebookAuth);
		}
#endif
	}

	// калбак авторизации
	void FacebookAuth() {
		if (FBController.CheckFbLogin) {
			animComp.SetBool("fbAuth", true);
			fbAuth = true;
		}
	}

	#endregion

	IEnumerator waitAndDisable() {
		yield return new WaitForSeconds(0.5f);
		waitHide = false;
		gameObject.SetActive(false);
	}

	public void ButtomToMap() {
		SceneManager.LoadScene("Map");
	}

}
