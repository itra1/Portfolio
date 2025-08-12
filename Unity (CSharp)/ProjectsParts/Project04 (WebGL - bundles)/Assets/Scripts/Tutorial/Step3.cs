using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Steps {
	public class Step3 : Step {
		public override int step {
			get { return 3; }
		}

		public override void StartStep() {
			base.StartStep();
      EnemyPanel.Instance.attackButton.interactable = false;

      SetPositionAttackBlock();
		}

		public void SetPositionAttackBlock() {

			TutorScreen.Instance.dialog.transform.SetParent(EnemyPanel.Instance.attackBlock.parent);
			TutorScreen.Instance.ShowDialogScreenPosition(EnemyPanel.Instance.attackBlock.anchoredPosition + Vector2.left * 50, new Vector2(1, 0), "Нажмите левой и правой кнопкой мыши на части тела, для обозначения ударов левой и правой рукой");
      EnemyPanel.Instance.helperRegionAttack.SetActive(true);

    }
		public void SetPositionButtonBlock() {

			TutorScreen.Instance.dialog.transform.SetParent(EnemyPanel.Instance.buttonBlock.parent);
			TutorScreen.Instance.ShowDialogScreenPosition(EnemyPanel.Instance.buttonBlock.anchoredPosition + new Vector2(-1,-1) * 50, new Vector2(1, 0), "Нажмите Атака");
      EnemyPanel.Instance.helperRegionAttack.SetActive(false);
    }

		public void SetAttack(BodyElement type, int button) {

      //if (button == 0) {
      //	PlayersManager.Instance.attackLeft = type;
      //} else {
      //	PlayersManager.Instance.attackRight = type;
      //}

      Debug.Log(PlayersManager.Instance.attackLeft + " : " + PlayersManager.Instance.attackRight);

      if (PlayersManager.Instance.attackLeft != BodyElement.none || PlayersManager.Instance.attackRight != BodyElement.none) {
        SetPositionButtonBlock();
        EnemyPanel.Instance.attackButton.interactable = true;
      } else if(PlayersManager.Instance.attackLeft == BodyElement.none && PlayersManager.Instance.attackRight == BodyElement.none) {
        SetPositionAttackBlock();
        EnemyPanel.Instance.attackButton.interactable = false;
      }

      //ExEvent.GameEvents.ChangeAttackTargets.Call(PlayersManager.Instance.attackLeft, PlayersManager.Instance.attackRight, BodyElement.none, BodyElement.none, BodyElement.none,
      //	BodyElement.none);
    }

		public void AttackButton() {

			SetPositionAttackBlock();

		}

	}
}