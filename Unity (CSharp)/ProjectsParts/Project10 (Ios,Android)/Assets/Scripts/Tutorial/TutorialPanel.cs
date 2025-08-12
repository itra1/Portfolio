/*
  Дисплейный Контроллер для отображения онформации по туториалу
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialPanel : PanelUiBase {

	public GameObject skipPanel;
	public GameObject playPanel;
	public GameObject runPanel;
	public GameObject jumpPanel;
	public GameObject jumpHole1Panel;
	public GameObject jumpHole2Panel;
	public GameObject enemyesPanel;
	public GameObject enemyes1Panel;
	public GameObject enemyes2Panel;
	public GameObject finishPanel;

	private bool skip;

	private void OnEnable() {
		SetLanuage();
		skipPanel.SetActive(false);
		playPanel.SetActive(false);
		runPanel.SetActive(false);
		jumpPanel.SetActive(false);
		jumpHole1Panel.SetActive(false);
		jumpHole2Panel.SetActive(false);
		enemyesPanel.SetActive(false);
		enemyes1Panel.SetActive(false);
		enemyes2Panel.SetActive(false);
		finishPanel.SetActive(false);

		Tutorial.TutorialController.OnEndTutorial += OnEndTutorial;

		//ChangeStepPhase(GameObject.Find("Tutorial").GetComponent<TutorialController>().stepTutorial);
		ChangeStepPhase(Tutorial.TutorialController.Instance.activeStep.step);

		Tutorial.TutorialController.ChangeStep += ChangeStepPhase;
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		Tutorial.TutorialController.ChangeStep -= ChangeStepPhase;
		Tutorial.TutorialController.OnEndTutorial -= OnEndTutorial;
	}

	void OnEndTutorial() {
		SkipTutorial();
	}

	void ChangeStepPhase(int step) {

		if (step == 1) {
			//playPanel.SetActive(true);
			//Tutorial.TutorialController.Instance.PlayTutorial();
		}

		if (step == 1) {
			if (gameObject == null || !gameObject.activeInHierarchy) return;
			playPanel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitAndShow(runPanel, 0.5f));
			StartCoroutine(WaitAndShow(skipPanel, 0.5f));
		}

		if (step == 2) {
			if(gameObject == null || !gameObject.activeInHierarchy) return;
			runPanel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(runPanel, 0.5f));
			StartCoroutine(WaitAndShow(jumpPanel, 0.5f));
		}

		if (step == 3) {
			if (gameObject == null || !gameObject.activeInHierarchy) return;
			jumpPanel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(jumpPanel, 0.5f));
			StartCoroutine(WaitAndShow(jumpHole1Panel, 0.5f));
		}

		if (step == 4) {
			if (gameObject == null || !gameObject.activeInHierarchy) return;
			jumpHole1Panel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(jumpHole1Panel, 0.5f));
			StartCoroutine(WaitAndShow(jumpHole2Panel, 0.5f));
		}

		if (step == 5) {
			if (gameObject == null || !gameObject.activeInHierarchy) return;
			jumpHole2Panel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(jumpHole2Panel, 0.5f));
			StartCoroutine(WaitAndShow(enemyesPanel, 0.5f));
		}

		if (step == 6) {
			if (gameObject == null || !gameObject.activeInHierarchy) return;
			enemyesPanel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(enemyesPanel, 0.5f));
			StartCoroutine(WaitAndShow(enemyes1Panel, 0.5f));
		}

		if (step == 7) {
			if (gameObject == null || !gameObject.activeInHierarchy) return;
			enemyes1Panel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(enemyes1Panel, 0.5f));
			StartCoroutine(WaitAndShow(enemyes2Panel, 0.5f));
		}

		if (step == 8) {
			if (gameObject == null || !gameObject.activeInHierarchy) return;
			enemyes2Panel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(enemyes2Panel, 0.5f));
			skipPanel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(skipPanel, 0.5f));
			StartCoroutine(WaitAndShow(finishPanel, 0.5f));
		}

		if (step == 9) {
			if (gameObject == null || !gameObject.activeInHierarchy) return;
			finishPanel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(finishPanel, 0.5f));
			StartCoroutine(WaitEndClose(gameObject, 0.5f));
		}

	}

	void SkipTutorial() {

		YAppMetrica.Instance.ReportEvent("Туториал: пропустить");

		GAnalytics.Instance.LogEvent("Туториал", "Активация", "Пропустить", 1);

		skipPanel.GetComponent<Animator>().SetTrigger("close");
		StartCoroutine(WaitEndClose(skipPanel, 0.5f));
		StartCoroutine(WaitEndClose(gameObject, 1));
		skip = true;
		//GameObject.Find("Tutorial").GetComponent<TutorialController>().SkipTutorial(true);
		//TutorialController.instant.SkipTutorial(true);

		CloseAllPanels();
	}

	public void SkipTutorialButton() {
		Tutorial.TutorialController.Instance.Skip();
	}

	/// <summary>
	/// Запускаем выполнение туториола
	/// </summary>
	public void PlayTutorial() {
		//Tutorial.TutorialController.Instance.PlayTutorial();
	}

	IEnumerator WaitEndClose(GameObject obj, float time) {
		yield return new WaitForSeconds(time);
		obj.SetActive(false);
	}


	void CloseAllPanels() {
		if (playPanel.activeInHierarchy) {
			playPanel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(playPanel, 0.5f));
		}
		if (runPanel.activeInHierarchy) {
			runPanel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(runPanel, 0.5f));
		}
		if (jumpPanel.activeInHierarchy) {
			jumpPanel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(jumpPanel, 0.5f));
		}
		if (jumpHole1Panel.activeInHierarchy) {
			jumpHole1Panel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(jumpHole1Panel, 0.5f));
		}
		if (jumpHole2Panel.activeInHierarchy) {
			jumpHole2Panel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(jumpHole2Panel, 0.5f));
		}
		if (enemyesPanel.activeInHierarchy) {
			enemyesPanel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(enemyesPanel, 0.5f));
		}
		if (enemyes1Panel.activeInHierarchy) {
			enemyes1Panel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(enemyes1Panel, 0.5f));
		}
		if (enemyes2Panel.activeInHierarchy) {
			enemyes2Panel.GetComponent<Animator>().SetTrigger("close");
			StartCoroutine(WaitEndClose(enemyes2Panel, 0.5f));
		}
	}


	IEnumerator WaitAndShow(GameObject deleg, float time) {
		yield return new WaitForSeconds(time);
		if (!skip)
			deleg.SetActive(true);
	}


	public Text skipBtn;
	public Text textArrowForMuve1;
	public Text textArrowForMuve2;
	public Text textJump1;
	public Text textJump2;
	public Text textMuveJump1;
	public Text textMuveJump2;
	public Text textMuveJump3;
	public Text textDoubleJump;
	public Text textMuveSpear;
	public Text textSpearDefend;
	public Text textGetWeaponAndKill;
	public Text textGoodWork;

	public GameObject[] ruGroup;
	public GameObject[] enGroup;

	void SetLanuage() {

		SystemLanguage activeType = LanguageManager.Instance.selectLanuage;

		if (activeType == SystemLanguage.Russian) {
			foreach (GameObject one in ruGroup)
				one.SetActive(true);
			foreach (GameObject one in enGroup)
				one.SetActive(false);
		} else {
			foreach (GameObject one in ruGroup)
				one.SetActive(false);
			foreach (GameObject one in enGroup)
				one.SetActive(true);
		}
	}
}
