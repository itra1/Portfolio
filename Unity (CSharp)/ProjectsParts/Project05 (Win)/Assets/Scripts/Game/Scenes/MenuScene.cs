using I2.Loc;
using it.Game.Managers;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Scenes
{
  public class MenuScene : SceneBehaviour
  {
	 private bool isProcess = false;

	 protected override void Start()
	 {
		base.Start();

		if (!it.UI.FirtMenu.FirstMenu.IS_LAST_SHOW)
		  DOVirtual.DelayedCall(3, ShowPanel);
		else
		{
		  GameManager.Instance.PlayBackgroundMusic("Menu");
		  ShowPanel();
		}
	 }

	 private void OnEnable()
	 {
		isProcess = false;
	 }

	 private void ShowPanel()
	 {
		  var panel = Game.Managers.UiManager.GetPanel<it.UI.FirtMenu.FirstMenu>();
		  panel.gameObject.SetActive(true);
		  panel.onGameButtonClick = () =>
		  {
			 var confirmPanel = Game.Managers.UiManager.GetPanel<it.UI.ConfirmDialog.ConfirmDialog>();
			 confirmPanel.SetData(LocalizationManager.GetTranslation("UI.ConfirmDialog.RemoveDataAndNewGame"),
				LocalizationManager.GetTranslation("UI.ConfirmDialog.Confirm"),
				LocalizationManager.GetTranslation("UI.ConfirmDialog.Cancel"),
				() =>
				{
				  DarkTonic.MasterAudio.MasterAudio.PlaySound("UI_Menu_Confirm");
				  GameManager.Instance.UserManager.NewGame();
				  UiManager.Instance.RemoveInstance(panel);
				  confirmPanel.Hide(1f);
				  GameManager.Instance.StartGame();

				}, () =>
				{
				  confirmPanel.Hide(.5f);
				}
			 );
			 confirmPanel.Show();


		  };
		panel.onPlayBackAudio = () =>
		{
		  GameManager.Instance.PlayBackgroundMusic("Menu");
		  DarkTonic.MasterAudio.MasterAudio.PlaylistMasterVolume = 0;

		  DOTween.To(() => DarkTonic.MasterAudio.MasterAudio.PlaylistMasterVolume, (x) => DarkTonic.MasterAudio.MasterAudio.PlaylistMasterVolume = x, 1, 3);

		  
		};
		  panel.onLoadSceneClick = (lvl) =>
		  {
			 UiManager.Instance.RemoveInstance(panel);
			 GameManager.Instance.StartGame(lvl);
		  };
		  panel.onContinueButtonClick = () =>
		  {
			 GameManager.Instance.UserManager.Load();
			 UiManager.Instance.RemoveInstance(panel);
			 GameManager.Instance.StartGame();
		  };
		  panel.onSettingsButtonClick = () =>
		  {
			 panel.HideMenuAnimated(1, () =>
			 {
				GameManager.Instance.GameSettings.OpenSettingsPanel(() => {
				  panel.ShowMenuAnimated(1);
				});
			 });
			 			 
		  };
		  panel.onAboutButtonClick = () =>
		  {

		  };
		  panel.onExitButtonClick = () =>
		  {
			 GameManager.Instance.QuitGame();
		  };

		  Cursor.visible = true;
		}
	 }

}