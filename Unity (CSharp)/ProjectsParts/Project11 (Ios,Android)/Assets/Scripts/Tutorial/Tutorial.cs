using System.Collections;
using System.Collections.Generic;
using GameCompany.Save;
using UnityEngine;

public class Tutorial : Singleton<Tutorial> {

	public bool deactive;

	private bool _isTutorial;

	public bool isTutorial {
		get { return _isTutorial; }
		set {
			_isTutorial = value;
		}
	}

	public bool isComplete;

	public int tutorialLevel;

  private void Start() { }

  public void Save() {

		var saveData = new TutorialSave() {
			complete = isComplete
		};
		PlayerPrefs.SetString("tutorial", Newtonsoft.Json.JsonConvert.SerializeObject(saveData));
	}

	public void Load() {
		if (PlayerPrefs.HasKey("tutorial")) {
			var loadData = Newtonsoft.Json.JsonConvert.DeserializeObject<TutorialSave>(PlayerPrefs.GetString("tutorial"));
			isComplete = loadData.complete;
		}
		Initialize();

	}

	public void Initialize() {

#if !UNITY_EDITOR
		deactive = false;
#endif

		if (isTutorial || isComplete || deactive) return;

		isTutorial = true;

	}

	public void ToGame() {

		StartCoroutine(StartGame());

	}

	IEnumerator StartGame() {
		var comp =
			PlayerManager.Instance.company.saveCompanies.Find(x => x.shortCompany == PlayerManager.Instance.company.actualCompany);

		while (comp == null || comp.bonusLevels.Count <= 0 || comp.bonusLevels[0].words == null)
			yield return null;

		comp.bonusLevels[0].isComplited = false;
		comp.bonusLevels[0].words.Clear();
		comp.bonusLevels[1].isComplited = false;
		comp.bonusLevels[1].words.Clear();


		GameManager.Instance.TutorualToGame(() => {
			FirebaseManager.Instance.LogEvent("start_tutorial");
		}, () => {
			tutorialLevel = 0;
			//PlayerManager.Instance.company.lastBonus++;
		}, () => {
      
			GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			//word.octopus.PlayShow();

			//Invoke("OnOpenOctopus",0.5f);
		});
	}

	public void FirstComplete() {

		GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
		word.PlaySalut();
		//OctopusController.Instance.Happy();
    
			tutorialLevel = 1;
			//PlayerManager.Instance.company.lastBonus++;
			//word.Init();
			//word.OnShow();
			word.StopSalut();
			//(UIManager.Instance.GetPanel(UiType.game) as PlayGamePlay).tutorHelperTest.SetCode("tutor.gatAllWords");

			if (Application.systemLanguage == SystemLanguage.Russian) {
				ShowLanguageCompany();
			} else {
				ShowWordTranslate();
			}


      
	}

	private void ShowWordTranslate() {
		
		GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
    //word.PlaySalut();

    LanguageUi lang = UIManager.Instance.GetPanel<LanguageUi>();
    lang.gameObject.SetActive(true);
    lang.View(false);
    lang.OnDeactive = () => {
			tutorialLevel = 1;
			//PlayerManager.Instance.company.lastBonus++;
			//GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			word.Init();
			word.OnShow();
			//word.StopSalut();
			UIManager.Instance.GetPanel<PlayGamePlay>().tutorHelperTest.SetCode("tutor.gatAllWords");

		};

	}

	private void ShowLanguageCompany() {
		LanguageUi panel = UIManager.Instance.GetPanel<LanguageUi>();
		panel.gameObject.SetActive(true);
		panel.View(true);

		UIManager.Instance.GetPanel<LanguageUi>().OnDeactive = () => {

			StartCoroutine(WaitShow());

		};
	}

	IEnumerator WaitShow() {
		yield return new WaitForFixedUpdate();
		ShowWordTranslate();
	}
	

	public void SecondComplete() {
		GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
		word.PlaySalut();
		//OctopusController.Instance.Happy();
			word.StopSalut();
			GameManager.Instance.EndTutorialGame();
			PlayerManager.Instance.company.lastBonus = 2;

	}

	void DeactiveSettings() {
		UIManager.Instance.GetPanel<SettingUi>().OnDeactive = null;
		GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
		word.Init();
		word.OnShow();
		OnOpenOctopus();
	}

	private void OnOpenOctopus() {
		ExEvent.TutorialEvents.LevelLoad.Call();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnLetterLoaded))]
	private void OnLetterLoaded(ExEvent.GameEvents.OnLetterLoaded load) {
		ExEvent.TutorialEvents.PlayMovePointer.Call();
	}

	public void EndTutorial() {
		FirebaseManager.Instance.LogEvent("complete_tutorial");
		PushManager.Instance.SubscribePush();
		isTutorial = false;
		isComplete = true;
		ExEvent.TutorialEvents.TutorialEnd.Call();
		PlayerManager.Instance.Save(true);
		Save();
	}

}

public class TutorialSave {
	public bool complete;
}