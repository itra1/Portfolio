using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial {
	public class Tutorial : Singleton<Tutorial> {

		public bool _isActive;
    public bool isActive {
      get {
        return _isActive;
      }
      set {
        _isActive = value;
      }
    }


    public List<Steps.Step> stepList;

		private int roundTime = 10;

		public Steps.Step activeStep { get; protected set; }
		private int stepActive = 0;

		public void StartTutorial() {
			StepComplete();
		}

		public void StepComplete() {

			stepActive++;
			
			if (activeStep != null)
				activeStep.gameObject.SetActive(false);

			activeStep = stepList.Find(x => x.step == stepActive);

			if (activeStep == null) {
				Complete();
			} else {
				activeStep.gameObject.SetActive(true);
				activeStep.StartStep();
			}
			
		}

    [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.StartBattle))]
    public void StartBattle(ExEvent.BattleEvents.StartBattle eventData) {
      if (!isActive) {
        isActive = eventData.battleStart.battle_info.map_id == 34;
        if (isActive)
          StartTutorial();
      }
    }

    public void Complete() {
			isActive = false;
			PlayerPrefs.SetInt("tutorial", 1);
			//todo что то отправляем на сервер о готовности
		}

		public BodyElement attackLeft = BodyElement.none;
		public BodyElement attackRight = BodyElement.none;
		public void SetAttack(BodyElement type, int button) {

			if(!isActive) return;
			if(stepActive != 3) return;
			
			(activeStep as Steps.Step3).SetAttack(type, button);
			
		}

		public void AttackButton() {

			if (!isActive) return;
			if (stepActive != 3) return;

			(activeStep as Steps.Step3).AttackButton();
		}

    public bool ClickReady(Cells.Cell cell) {

      if (stepActive == 1) {
        return (activeStep as Steps.Step1).MoveReady(cell);
      }

      if (stepActive == 2) {
        return (activeStep as Steps.Step2).MoveReady(cell);
      }

      if (stepActive == 3) return false;

      return true;

    }

	}
}