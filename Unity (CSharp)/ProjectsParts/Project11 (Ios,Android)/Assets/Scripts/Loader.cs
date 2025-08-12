using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loader : ExEvent.EventBehaviour {

	public Animation anim;
	public Text text;
	private AsyncOperation async;
	public bool loadMap;

	private bool isStarted;
	
	private void LoadingComplete(AsyncOperation oper) {
		async.completed -= LoadingComplete;
		//StopAllCoroutines();
		SceneManager.UnloadSceneAsync(0);
		
	}

	private void Start() {
//#if UNITY_EDITOR
		StartCoroutine(StartLoad());
//#endif
	}
	
	[ExEvent.ExEventHandler(typeof(ExEvent.LoadEvents.LoadProgress))]
	public void OnLoadProgress(ExEvent.LoadEvents.LoadProgress eventData) {

		if (eventData.loadProgress >= 1 && !isStarted) {
			isStarted = true;
			StartCoroutine(StartLoad());
		}
	}

	IEnumerator StartLoad() {
		yield return new WaitForSeconds(0.1f);
		if (loadMap) {
      //Debug.Log(Application.dataPath + "/Scenes/Game.bytes");
      //byte[] scene = DevXUnity.LoadEncryptedResourceBinary(@"Game", null);
      //File.WriteAllBytes(Application.streamingAssetsPath+"/game.unity", scene);
      //Resources.Load<Scene>(Application.streamingAssetsPath + "/game.unity");
      GameManager.gamePhase = GamePhase.menu;
			async = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
			//SceneManager.
			async.completed += LoadingComplete;
		}

		string textLoading = LanguageManager.GetTranslate("base.loader");
		//string text = "LOADING";
		string useText = "";
		int pointCount = 0;
		
		while (true) {
			useText = textLoading;
			for (int i = 0; i < pointCount; i++)
				useText += ".";

			this.text.text = useText.ToUpper();
			yield return new WaitForSeconds(0.3f);
			pointCount++;
			if (pointCount >= 4)
				pointCount = 0;
		}
	}

	private Scene ByteArrayToObject(byte[] arrBytes) {

		MemoryStream memStream = new MemoryStream(arrBytes);
		BinaryFormatter binForm = new BinaryFormatter();
		//memStream.Write(arrBytes, 0, arrBytes.Length);
		//memStream.Seek(0, SeekOrigin.Begin);
		memStream.Position = 0;
		Scene obj = (Scene)binForm.Deserialize(memStream);
		return obj;
	}

}
