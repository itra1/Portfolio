using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

public class GamePlayBattle : EventBehaviour {

	public RectTransform miniMap;
	public GameObject enemyPanel;
  public GameObject helpPanel;

	protected override void Awake() {
		base.Awake();
		enemyPanel.SetActive(true);
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.PlayerSelect))]
	public void EnemySelect(ExEvent.GameEvents.PlayerSelect playerSelect) {
		enemyPanel.SetActive(true);
		enemyPanel.GetComponent<InfoPanel>().SetSelectPlayer(playerSelect.player);
		
		//miniMap.anchoredPosition = new Vector2(-200,0);
	}

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.StartBattle))]
  public void StartBattle(ExEvent.BattleEvents.StartBattle playerSelect) {
    if (playerSelect.battleStart.battle_info.map_id == 34) {
      ShowHelpPanel();
    }

    //miniMap.anchoredPosition = new Vector2(-200,0);
  }

  

  public void DeactivePanel() {
		//enemyPanel.SetActive(false);
		//miniMap.anchoredPosition = new Vector2(0, 0);
	}

	public void ClickedRegion() {
		
		if (Input.GetMouseButtonDown(0)) {

		}

		if (Input.GetMouseButtonUp(0)) {
			ExEvent.GameEvents.MapClick.Call(Input.mousePosition);
		}

		if (Input.GetMouseButtonDown(1)) {
			CameraController.Instance.scroollButtonMove = true;
		}

		if (Input.GetMouseButtonUp(1)) {
			CameraController.Instance.scroollButtonMove = false;
		}

		if (Input.GetMouseButtonDown(2)) {
			CameraController.Instance.scroollButtonMove = true;
		}

		if (Input.GetMouseButtonUp(2)) {
			CameraController.Instance.scroollButtonMove = false;
		}

	}

	public void MouseScroll() {
		CameraController.Instance.Scroll(Input.mouseScrollDelta.y*5);
	}

	#region Навигация по карте

	//public void MoveForwardButtonDown() {
	//	CameraController.Instance.forwardMove = 1;
	//}
	//public void MoveForwardButtonUp() {
	//	CameraController.Instance.forwardMove = 0;
	//}
	//public void MoveBackButtonDown() {
	//	CameraController.Instance.forwardMove = -1;
	//}
	//public void MoveBackButtonUp() {
	//	CameraController.Instance.forwardMove = 0;
	//}
	//public void MoveUpButtonDown() {
	//	CameraController.Instance.verticalMove = 1;
	//}
	//public void MoveUpButtonUp() {
	//	CameraController.Instance.verticalMove = 0;
	//}
	//public void MoveLeftButtonDown() {
	//	CameraController.Instance.horizontalMove = -1;
	//}
	//public void MoveLeftButtonUp() {
	//	CameraController.Instance.horizontalMove = 0;
	//}
	//public void MoveRightButtonDown() {
	//	CameraController.Instance.horizontalMove = 1;
	//}
	//public void MoveRightButtonUp() {
	//	CameraController.Instance.horizontalMove = 0;
	//}
	//public void MoveDownButtonDown() {
	//	CameraController.Instance.verticalMove = -1;
	//}
	//public void MoveDownButtonUp() {
	//	CameraController.Instance.verticalMove = 0;
	//}
	//public void RotateUpButtonDown() {
	//	CameraController.Instance.verticalRotate = 1;
	//}
	//public void RotateUpButtonUp() {
	//	CameraController.Instance.verticalRotate = 0;
	//}
	//public void RotateLeftButtonDown() {
	//	CameraController.Instance.horizontalRotate = -1;
	//}
	//public void RotateLeftButtonUp() {
	//	CameraController.Instance.horizontalRotate = 0;
	//}
	//public void RotateRightButtonDown() {
	//	CameraController.Instance.horizontalRotate = 1;
	//}
	//public void RotateRightButtonUp() {
	//	CameraController.Instance.horizontalRotate = 0;
	//}
	//public void RotateDownButtonDown() {
	//	CameraController.Instance.verticalRotate = -1;
	//}
	//public void RotateDownButtonUp() {
	//	CameraController.Instance.verticalRotate = 0;
	//}
	//public void ZoomPlusButtonDown() {
	//	CameraController.Instance.zoom = 1;
	//}
	//public void ZoomPlusButtonUp() {
	//	CameraController.Instance.zoom = 0;
	//}
	//public void ZoomMinusButtonDown() {
	//	CameraController.Instance.zoom = -1;
	//}
	//public void ZoomMinusButtonUp() {
	//	CameraController.Instance.zoom = 0;
	//}
#endregion

	/// <summary>
	/// показать лог боя
	/// </summary>
	public void ShowBattleLog() {
		BattleManager.Instance.OpenBattleLog();
	}
	/// <summary>
	/// Изменение качества рендера
	/// </summary>
	public void QualityButton() {
		
		QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel() == 1? 0 : 1);

	}

	/// <summary>
	/// Посветка друзей
	/// </summary>
	public void SetFriendLight() {
		PlayersManager.Instance.SetFriendLight();
	}

	/// <summary>
	/// Подсветка врагов
	/// </summary>
	public void SetEnemyLight() {
		PlayersManager.Instance.SetEnemyLight();
	}

	/// <summary>
	/// Вызов наемников
	/// </summary>
	public void MercenaryBtn() {
		BrowserContact.Instance.OnOpenMercenaryList();
	}

	public void PlayerToCamera() {
		PlayersManager.Instance.MainPlayerToCentrCamera();
	}


  public void ShowHelpPanel() {
    helpPanel.SetActive(true);
  }

}
