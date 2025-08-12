using System.Collections;
using UnityEngine;

public class Splash : MonoBehaviour {

	public string nextScene;

	private Animation _anim;

	private Animation anim {
		get {
			if (_anim == null)
				_anim = GetComponent<Animation>();
			return _anim;
		}
	}

	void Start() {

		Play();

//		Invoke("StartMove", 1);
//		Invoke("LoadRun", 5);
//#if UNITY_EDITOR
//		LoadMenu();
//#endif

	}
	
	private void Play() {
		anim.Play("play");

	}


	//void StartMove() {
	//	Handheld.PlayFullScreenMovie("Splash_KB_Games_S.mp4", Color.white, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
	//}

	//bool isPause;

	//void OnApplicationPause(bool pauseStatus) {
	//	if (pauseStatus) {
	//		isPause = pauseStatus;
	//	} else {
	//		if (isPause) {
	//			if (IsInvoking("LoadRun")) CancelInvoke("LoadRun");
	//			LoadMenu();
	//			isPause = false;
	//		}
	//	}

	//}

	public void AnimationComplete() {
		LoadMenu();
	}

	void LoadMenu() {
		GameManager.LoadScene(nextScene, true);
	}

}
