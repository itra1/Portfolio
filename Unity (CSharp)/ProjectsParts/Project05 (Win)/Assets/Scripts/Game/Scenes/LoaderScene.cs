using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Scenes
{
  public class LoaderScene : SceneBehaviour
  {
	 it.UI.SceneLoader.SceneLoader _panel;

	 protected override void Start()
	 {
		ShowLoader();
	 }


	 private void OnDestroy()
	 {
		_panel.gameObject.SetActive(false);

	 }
	 private void ShowLoader()
	 {
		_panel = Game.Managers.UiManager.GetPanel<it.UI.SceneLoader.SceneLoader>();

		_panel.gameObject.SetActive(true);

		//panel.onLoadComplete = () =>
		//{
		//  panel.gameObject.SetActive(false);
		//  //Game.Managers.GameManager.Instance.SceneManager.UnloadSceneAsync("_Begin");
		//};

		//DOVirtual.DelayedCall(0.1f, ShowLoader);

		//Game.Managers.GameManager.Instance.SceneManager.LoadSceneAsync("Menu");
	 }
  }
}