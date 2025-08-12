/*
  Контроллер выполнения професса обучения

  Шаги:
  1 - Обущение по свижению влево и вправо
  2 - Перепрыгивание камня
  3 - Прыжок через яму
  4 - Прыжок через больгую яму
  5 - Увернуться от копья
  6 - Отбить копье
  7 - Убить врага
  8 - Финиш

*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial {
	public class TutorialController : Singleton<TutorialController> {
		
		public List<Steps.Step> stepList;

		public Steps.Step activeStep;
		private int stepActive = 0;
		public static bool isActive = false;
		private RunnerPhase runnerPhase;
		
		public static event System.Action<int> ChangeStep;
		public static event System.Action OnEndTutorial;

		private void OnEnable() {
			RunnerController.OnChangeRunnerPhase += ChangePhase;
		}
		
		private void OnDisable() {
			RunnerController.OnChangeRunnerPhase -= ChangePhase;
		}

		void ChangePhase(RunnerPhase runnerPhase) {

			if (runnerPhase == RunnerPhase.tutorial) {
				isActive = true;
				StepComplete();
        TutorialPanelShow(true);

      }
			this.runnerPhase = runnerPhase;
		}
		
		public void Skip() {
			isActive = false;
      TutorialManager.Instance.Istutorial = true;

			if (OnEndTutorial != null) OnEndTutorial();

			RunnerController.Instance.StartPlay();

			//if (ChangeStep != null) ChangeStep(activeStep.step);
		}



    TutorialPanel tutoriaPanel;

    void TutorialPanelShow(bool isShow) {

      if (tutoriaPanel == null)
        tutoriaPanel = UiController.ShowUi<TutorialPanel>();

      tutoriaPanel.gameObject.SetActive(isShow);
      //Tutorial.TutorialController.Instance.PlayTutorial();
    }


    public void StepComplete() {

			stepActive++;

			if (ChangeStep != null) ChangeStep(stepActive);

			if (activeStep != null)
				activeStep.gameObject.SetActive(false);

			activeStep = stepList.Find(x => x.step == stepActive);

			if (activeStep == null) {
				Skip();
			}
			else {
				activeStep.gameObject.SetActive(true);
				activeStep.StartStep();
			}


		}

	}

}