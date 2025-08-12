using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tutorial.Steps {

	public class Step2 : Step {
		public override int step {
			get { return 2; }
		}

		private void Update() {

			if (isVisible)
				TutorScreen.Instance.UpdatePositionWord(dialogPosition + Vector3.one * 2);
		}

		private Vector3 dialogPosition;
		private bool isVisible = false;

		public override void StartStep() {
			base.StartStep();

			if (CheckSelected()) {
				StepComplete();
				return;
			}

			PlayerBehaviour player =
				GameObject.FindObjectsOfType<PlayerBehaviour>()
					.ToList()
					.Find(x => x.name != PlayersManager.Instance.mainPlayer.name);

			dialogPosition = player.transform.position;

			TutorScreen.Instance.ShowDialogWorldPosition(dialogPosition + Vector3.one * 2, Vector2.zero, "Нажмите на противника, чтобы выбрать");

			isVisible = true;
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.PlayerSelect))]
		public void PlayerSelect(ExEvent.GameEvents.PlayerSelect eventData) {

			if (CheckSelected()) {
				isVisible = false;
				TutorScreen.Instance.HideDialog();
				StepComplete();
			}

		}


    public bool MoveReady(Cells.Cell cell) {

      if (PlayersManager.Instance.mainPlayer._actualCell.gridX == 4 && PlayersManager.Instance.mainPlayer._actualCell.gridZ == 6) {
        return cell.gridX == 5 && cell.gridZ == 6;
      }
      return false;

    }

    private bool CheckSelected() {
			if (PlayersManager.Instance.selectEnemyPlayer == null)
				return false;

			return PlayersManager.Instance.selectEnemyPlayer != PlayersManager.Instance.mainPlayer;

		}

	}
}