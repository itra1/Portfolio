using System.Collections;
using System.Collections.Generic;
using it.Game.Managers;
using UnityEngine;

namespace it.Game.Scenes
{
  /// <summary>
  /// Стартовая сцена
  /// </summary>
  public class BeginScene : SceneBehaviour
  {
	 protected override void Start()
	 {
		ShowLogosPanel();
		//GameManager.Instance.PlayBackgroundMusic("Menu");
	 }

	 private void ShowLogosPanel()
	 {
		var panel = Game.Managers.UiManager.GetPanel<it.UI.FirstLogo.FirstLogo>();
		panel.gameObject.SetActive(true);
		panel.onLogoShowComplete = () =>
		{
		  panel.gameObject.SetActive(false);
		  Game.Managers.UiManager.Instance.RemoveInstance(panel);
		  Game.Managers.GameManager.Instance.SceneManager.LoadScene("Menu", false);
		};
	 }

  }
}