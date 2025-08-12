using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Загрузчик
/// 
/// </summary>
public class Loader : EventBehaviour {

	private void Start() {

		StartCoroutine(LoadLevel());
	}

	private AsyncOperation sceneLoadOperation;

	IEnumerator LoadLevel() {
		
		yield return new WaitForEndOfFrame();

		sceneLoadOperation = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
		
		while (!sceneLoadOperation.isDone) {
			yield return null;

			//ExEvent.LoadEvents.LoadProgress.Call("Подготовка сцены", sceneLoadOperation.progress/2);
		}

		SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
		
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.LoadAllModels))]
	void MapLoadComplited(ExEvent.BattleEvents.LoadAllModels result) {
		SceneManager.UnloadSceneAsync("Loader");
	}

	//IEnumerator DopLevel() {

	//	sceneLoadOperation = SceneManager.LoadSceneAsync("map_1(1)", LoadSceneMode.Additive);

	//	while (!sceneLoadOperation.isDone) {
	//		yield return null;

	//		ExEvent.LoadEvents.LoadProgress.Call( 50 + (sceneLoadOperation.progress/2));
	//	}
	//}

}
