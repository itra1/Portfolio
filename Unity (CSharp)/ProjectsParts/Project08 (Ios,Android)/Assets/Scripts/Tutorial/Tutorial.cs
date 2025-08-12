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

		while (comp == null || comp.bonusLocation == null || comp.bonusLocation.levels.Count <= 0 || comp.bonusLocation.levels[0].words == null)
			yield return null;

		comp.bonusLocation.levels[0].isComplited = false;
		comp.bonusLocation.levels[0].words.Clear();
		comp.bonusLocation.levels[1].isComplited = false;
		comp.bonusLocation.levels[1].words.Clear();


		GameManager.Instance.TutorualToGame(() => {
			FirebaseManager.Instance.LogEvent("start_tutorial");
		}, () => {
			tutorialLevel = 0;
			//PlayerManager.Instance.company.lastBonus++;
		}, () => {

			PetDialogs.Instance.ShowDialog(PetDialogType.tutorStart, DeactiveSettings);
			GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			word.octopus.PlayShow();

			//Invoke("OnOpenOctopus",0.5f);
		});
	}

	public void FirstComplete() {

		GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
		word.PlaySalut();
		OctopusController.Instance.Happy();

		PetDialogs.Instance.ShowDialog(PetDialogType.tutor1End, () => {
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



			//UIManager.Instance.GetPanel(UiType.language).gameObject.SetActive(true);

			//if (Application.systemLanguage == SystemLanguage.Russian) {
			//	(UIManager.Instance.GetPanel(UiType.language) as LanguageUi).View(true);
			//} else {
			//	(UIManager.Instance.GetPanel(UiType.language) as LanguageUi).View(false);
			//}

			//UIManager.Instance.GetPanel(UiType.language).OnDeactive = () => {

			//	if (Application.systemLanguage == SystemLanguage.Russian) {
			//		StartCoroutine(WaitShow());

			//	} else {
			//		tutorialLevel = 1;
			//		//PlayerManager.Instance.company.lastBonus++;
			//		//GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			//		word.Init();
			//		word.OnShow();
			//		//word.StopSalut();
			//		(UIManager.Instance.GetPanel(UiType.game) as PlayGamePlay).tutorHelperTest.SetCode("tutor.gatAllWords");
			//	}

			//};

		});
	}

	private void ShowWordTranslate() {
		
		GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
		//word.PlaySalut();

		UIManager.Instance.GetPanel(UiType.language).gameObject.SetActive(true);
		(UIManager.Instance.GetPanel(UiType.language) as LanguageUi).View(false);
		UIManager.Instance.GetPanel(UiType.language).OnDeactive = () => {
			tutorialLevel = 1;
			//PlayerManager.Instance.company.lastBonus++;
			//GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			word.Init();
			word.OnShow();
			//word.StopSalut();
			(UIManager.Instance.GetPanel(UiType.game) as PlayGamePlay).tutorHelperTest.SetCode("tutor.gatAllWords");

		};

	}

	private void ShowLanguageCompany() {
		LanguageUi panel = (LanguageUi)UIManager.Instance.GetPanel(UiType.language);
		panel.gameObject.SetActive(true);
		panel.View(true);

		UIManager.Instance.GetPanel(UiType.language).OnDeactive = () => {

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
		OctopusController.Instance.Happy();
		PetDialogs.Instance.ShowDialog(PetDialogType.tutorial2End, () => {
			word.StopSalut();
			GameManager.Instance.EndTutorialGame();
			PlayerManager.Instance.company.lastBonus = 2;
		});
	}

	void DeactiveSettings() {
		UIManager.Instance.GetPanel(UiType.setting).OnDeactive = null;
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