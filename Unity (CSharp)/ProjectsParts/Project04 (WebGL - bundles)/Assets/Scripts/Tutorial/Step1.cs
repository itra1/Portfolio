using System;
using System.Collections;
using System.Collections.Generic;
using Cells;
using UnityEngine;

namespace Tutorial.Steps {
	public class Step1 : Step {

		public override int step {
			get { return 1; }
		}

    ProjectorBehaviour pb;

    private int moveStep = 0;

		private void OnEnable() {
      EnemyPanel.Instance.attackButton.interactable = false; 
      EnemyPanel.Instance.endButton.interactable = false;
    }

		private void OnDisable() {
			PlayersManager.Instance.mainPlayer.moveBehaviour.OnMoveEnd -= OnMoveEnd;
      PlayersManager.Instance.mainPlayer.moveBehaviour.OnMoveStart -= OnMoveStart;
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.LoadAllModels))]
    private void LoadAllModels(ExEvent.BattleEvents.LoadAllModels eventData) {
      PlayersManager.Instance.mainPlayer.moveBehaviour.OnMoveEnd += OnMoveEnd;
      PlayersManager.Instance.mainPlayer.moveBehaviour.OnMoveStart += OnMoveStart;
      OnMoveEnd();
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.LoadNewCells))]
    private void LoadNewCells(ExEvent.BattleEvents.LoadNewCells eventData) {

      if (PlayersManager.Instance.mainPlayer._actualCell.gridX == 2 && PlayersManager.Instance.mainPlayer._actualCell.gridZ == 3) {

        cell = MapManager.Instance.map._cellList.Find(x => x.gridX == 3 && x.gridZ == 4);
        pb = GetInstanceProjector();
        pb.gameObject.SetActive(true);
        pb.Init(cell);
        pb.SetScale(MapManager.Instance.map.cellSize);
        pb.SetCell(cell.position, CellType.friendSelect);
        TutorScreen.Instance.ShowDialogWorldPosition(cell.position + Vector3.one * 2, Vector2.zero, "Нажмите на выделенную ячейку, чтобы совершить ход");
        isVisible = true;

        return;
      }

      if (PlayersManager.Instance.mainPlayer._actualCell.gridX == 3 && PlayersManager.Instance.mainPlayer._actualCell.gridZ == 4) {

        cell = MapManager.Instance.map._cellList.Find(x => x.gridX == 3 && x.gridZ == 5);
        pb = GetInstanceProjector();
        pb.gameObject.SetActive(true);
        pb.Init(cell);
        pb.SetScale(MapManager.Instance.map.cellSize);
        pb.SetCell(cell.position, CellType.friendSelect);
        TutorScreen.Instance.ShowDialogWorldPosition(cell.position + Vector3.one * 2, Vector2.zero, "Нажмите на выделенную ячейку, чтобы совершить ход");
        isVisible = true;

        return;
      }

      if (PlayersManager.Instance.mainPlayer._actualCell.gridX == 3 && PlayersManager.Instance.mainPlayer._actualCell.gridZ == 5) {

        cell = MapManager.Instance.map._cellList.Find(x => x.gridX == 4 && x.gridZ == 6);
        pb = GetInstanceProjector();
        pb.gameObject.SetActive(true);
        pb.Init(cell);
        pb.SetScale(MapManager.Instance.map.cellSize);
        pb.SetCell(cell.position, CellType.friendSelect);
        TutorScreen.Instance.ShowDialogWorldPosition(cell.position + Vector3.one * 2, Vector2.zero, "Нажмите на выделенную ячейку, чтобы совершить ход");
        isVisible = true;

        return;
      }
    }

    private void Update() {

			if(cell != null && isVisible)
				TutorScreen.Instance.UpdatePositionWord(cell.position + Vector3.one * 2);
		}

		private Cell cell;
		private bool isVisible = false;

    private ProjectorBehaviour GetInstanceProjector() {
      if(pb == null)
        pb = ProjectorManager.Instance.GetProjector(true);
      return pb;
    }


		private void NextMove() {
      
      if(PlayersManager.Instance.mainPlayer._actualCell.gridX == 2 && PlayersManager.Instance.mainPlayer._actualCell.gridZ == 3) {

        //cell = MapManager.Instance.map._cellList.Find(x => x.gridX == 3 && x.gridZ == 4);
        //pb = GetInstanceProjector();
        //pb.gameObject.SetActive(true);
        //pb.Init(cell);
        //pb.SetScale(MapManager.Instance.map.cellSize);
        //pb.SetCell(cell.position, CellType.friendSelect);
        //TutorScreen.Instance.ShowDialogWorldPosition(cell.position + Vector3.one * 2, Vector2.zero, "Нажмите на выделенную ячейку, что бы совершить ход");
        //isVisible = true;

        return;
      }

      if (PlayersManager.Instance.mainPlayer._actualCell.gridX == 3 && PlayersManager.Instance.mainPlayer._actualCell.gridZ == 4) {

        //cell = MapManager.Instance.map._cellList.Find(x => x.gridX == 3 && x.gridZ == 5);
        //pb = GetInstanceProjector();
        //pb.gameObject.SetActive(true);
        //pb.Init(cell);
        //pb.SetScale(MapManager.Instance.map.cellSize);
        //pb.SetCell(cell.position, CellType.friendSelect);
        //TutorScreen.Instance.ShowDialogWorldPosition(cell.position + Vector3.one * 2, Vector2.zero, "Нажмите на выделенную ячейку, что бы совершить ход");
        //isVisible = true;

        return;
      }

      if (PlayersManager.Instance.mainPlayer._actualCell.gridX == 3 && PlayersManager.Instance.mainPlayer._actualCell.gridZ == 5) {

        //cell = MapManager.Instance.map._cellList.Find(x => x.gridX == 4 && x.gridZ == 6);
        //pb = GetInstanceProjector();
        //pb.gameObject.SetActive(true);
        //pb.Init(cell);
        //pb.SetScale(MapManager.Instance.map.cellSize);
        //pb.SetCell(cell.position, CellType.friendSelect);
        //TutorScreen.Instance.ShowDialogWorldPosition(cell.position + Vector3.one * 2, Vector2.zero, "Нажмите на выделенную ячейку, что бы совершить ход");
        //isVisible = true;

        return;
      }

      isVisible = false;
      TutorScreen.Instance.HideDialog();
      StepComplete();
      
		}

		public override void StartStep() {
			base.StartStep();
			OnMoveEnd();
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.MapClick))]
		public void MapClick(ExEvent.GameEvents.MapClick eventData) {

			if(!gameObject.activeInHierarchy) return;

			Cell cell = CellDrawner.Instance.GetGrideByPoint(GameManager.Instance.GetMapPosition(eventData.position));
			if (cell == null) return;

			switch (moveStep) {
				case 1:
					if(cell.gridX != 15 || cell.gridZ != 17)
						return;
					break;
				case 2:
					if (cell.gridX != 16 || cell.gridZ != 18)
						return;
					break;
				case 3:
					if (cell.gridX != 16 || cell.gridZ != 19)
						return;
					break;
				default:
					StepComplete();
					break;
			}

			isVisible = false;
			TutorScreen.Instance.HideDialog();
			//PlayersManager.Instance.ClickCell(cell);
			PlayersManager.Instance.mainPlayer.moveBehaviour.MoveToCell(cell);

		}

    public bool MoveReady(Cells.Cell cell) {

      if (PlayersManager.Instance.mainPlayer._actualCell.gridX == 2 && PlayersManager.Instance.mainPlayer._actualCell.gridZ == 3) {
        return cell.gridX == 3 && cell.gridZ == 4;
      }

      if (PlayersManager.Instance.mainPlayer._actualCell.gridX == 3 && PlayersManager.Instance.mainPlayer._actualCell.gridZ == 4) {
        return cell.gridX == 3 && cell.gridZ == 5;
      }

      if (PlayersManager.Instance.mainPlayer._actualCell.gridX == 3 && PlayersManager.Instance.mainPlayer._actualCell.gridZ == 5) {
        return cell.gridX == 4 && cell.gridZ == 6;
      }
      return false;
    }

		private void OnMoveEnd() {
      if (PlayersManager.Instance.mainPlayer == null) return;
			moveStep++;
			NextMove();
    }
    private void OnMoveStart() {
      TutorScreen.Instance.HideDialog();
      isVisible = false;
    }

  }
}