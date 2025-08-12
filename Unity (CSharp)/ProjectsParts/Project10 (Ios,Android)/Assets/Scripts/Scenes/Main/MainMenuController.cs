using UnityEngine;
using ExEvent;

/// <summary>
/// Controller main menu
/// </summary>
public class MainMenuController : EventBehaviour {

	public AudioClip menyClip;                // Музыка меню

	void Start() {
		Application.targetFrameRate = 30;
		//ShowPage();
		AudioManager.BackMusic(menyClip, AudioMixerTypes.music);
	}

	private void OnEnable() {
		MenuGamePlayShow();
	}

	void ShowPage() {
   
   FillBlack panel = FillBlack.Instance;
    panel.gameObject.SetActive(true);
    panel.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.open, Vector3.zero, () => {
      Destroy(panel.gameObject);
    });
  }

  void MenuGamePlayShow() {
		MainMenuView view = UiController.ShowUi<MainMenuView>();
		view.gameObject.SetActive(true);
		view.OnSelectGameMode = OnSelectGameMode;
		//view.OnSurvival = OnSurvival;
		view.OnGameCenter = OnGameCenter;
	}

	void OnSelectGameMode(GameMode gameMode, GameLocation gameLocation) {
		GameManager.Instance.OnSelectGameMode(gameMode, gameLocation);
	}
	
	public void OnGameCenter() {
		UiController.ClickButtonAudio();
		GameCenterController.GameCenterLeaderBoard();
	}
	
}
