using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSceneHelper31 : ExEvent.EventBehaviour {

	public PlaneAnim planeAnim;
	//private bool isActive = false;

	private void Start() {
		planeAnim.OnFrame = Step;
	}

	public void Step(int numStep) {

		ZbCatScene.CatSceneManager.Instance.NextScene(true);

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.StartCatScene))]
	void StartCastScene(ExEvent.CatSceneEvent.StartCatScene cs) {
		if (cs.id == "31") {
			planeAnim.gameObject.SetActive(true);
		}
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.StartCatFrame))]
	void StartCastFrame(ExEvent.CatSceneEvent.StartCatFrame cs) {
		if (cs.id == "31" && (cs.pageNum == 1 || cs.pageNum == 2 || cs.pageNum == 3 || cs.pageNum == 4 || cs.pageNum == 5))
			planeAnim.Play();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.EndCatFrame))]
	void EndCastFrame(ExEvent.CatSceneEvent.EndCatFrame cs) {

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.EndCatScene))]
	void EndCastScene(ExEvent.CatSceneEvent.EndCatScene cs) {
		planeAnim.gameObject.SetActive(false);
		Destroy(gameObject);
		return;

	}

	

}
