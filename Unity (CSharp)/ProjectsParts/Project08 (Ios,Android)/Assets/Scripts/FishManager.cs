using System;
using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(FishManager))]
public class FishManagerEditor: Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if (GUILayout.Button("Generate")) {
			((FishManager)target).CreateInstance();
		}
	}
}

#endif

public class FishManager : Singleton<FishManager> {

	public GameObject fishPrefab;
	public FishGetEffect fishEffectPrefab;
	private List<FishBehaviour> instanceList = new List<FishBehaviour>();
	private List<FishGetEffect> effectsList = new List<FishGetEffect>();
	public Rect rect;

	private bool isDownload = false;
	private DateTime lastGenerate = DateTimeOffset.UtcNow.DateTime.AddMinutes(-95);

	private int _fishCount;
	
	public void Init() {
		NetManager.Instance.GetTime(DownloadComplete, () => { });
	}

	public void DownloadComplete(string timeDownload) {
		isDownload = true;
		var serverTime = DateTime.Parse(timeDownload);

		var allMinutDelta = (serverTime - lastGenerate).TotalMinutes;

		int addCount = (int) (allMinutDelta / 30);

		lastGenerate = lastGenerate.AddMinutes(30 * addCount);

		_fishCount += addCount;

		if (_fishCount > 7)
			_fishCount = 7;

		Generate();

		if (_fishCount < 7) {
			_genCorotine = StartCoroutine(GenerateCoroutine(lastGenerate.AddMinutes(30)- serverTime));
		}
	}

	private void Generate() {

		int actual = instanceList.FindAll(x => x.gameObject.activeInHierarchy).Count;
		while (actual < _fishCount) {
			CreateInstance();
			actual++;
		}

	}

	private Coroutine _genCorotine;
	IEnumerator GenerateCoroutine(TimeSpan waitSecond) {
		//Debug.Log("Следующая рыбка через " + (float)waitSecond.TotalSeconds);
		yield return new WaitForSecondsRealtime((float)waitSecond.TotalSeconds);
		while (_fishCount < 7) {
			CreateInstance();
			_fishCount++;
			yield return new WaitForSecondsRealtime(30);
		}
	}

	public FishSave Save() {
		return new FishSave {
			count = _fishCount,
			time = lastGenerate.ToString()
		};
	}

	public void Load(FishSave saveData) {
		_fishCount = saveData.count;
		lastGenerate = DateTime.Parse(saveData.time);
	}

	private int coinsReady;
	private bool coinsPanelShow;

	public void ClickFish(FishBehaviour fish) {
		fish.gameObject.SetActive(false);

		bool neqStartCor = false;
		if (_fishCount == 7)
			neqStartCor = true;
		_fishCount--;

		if (neqStartCor) {
			lastGenerate = DateTimeOffset.UtcNow.DateTime;
			_genCorotine = StartCoroutine(GenerateCoroutine(lastGenerate.AddMinutes(30) - lastGenerate));
		}

		MenuUi menu = (MenuUi)UIManager.Instance.GetPanel(UiType.menu);
		
		FishGetEffect effect = GetEffect();
		effect.transform.position = fish.transform.position;
		effect.SetCoinsTransform(menu.coinsTransform);
		effect.gameObject.SetActive(true);
		GameEvents.OnFishTouch.Call();
	}

	public void GenerateCoins() {
		MenuUi menu = (MenuUi)UIManager.Instance.GetPanel(UiType.menu);
		coinsReady++;

		if (!coinsPanelShow) {
			coinsPanelShow = true;
			menu.ShowCoinsPanel();
		}
	}

	public void CoinsMoveComplete() {
		coinsReady--;
		//PlayerManager.Instance.coins += UnityEngine.Random.Range(1,4);
		PlayerManager.Instance.coins += 1;

		MenuUi menu = (MenuUi)UIManager.Instance.GetPanel(UiType.menu);
		menu.AddCoinsPanel();

		if (coinsReady <= 0) {
			menu.HideCoinsPanel();
			coinsPanelShow = false;
		}
	}

	private FishGetEffect GetEffect() {
		FishGetEffect effect = effectsList.Find(x => !x.gameObject.activeInHierarchy);
		if (effect == null) {
			GameObject inst = Instantiate(fishEffectPrefab.gameObject);
			effect = inst.GetComponent<FishGetEffect>();
			effectsList.Add(effect);
		}
		return effect;
	}

	public void CreateInstance() {

		FishBehaviour fish = instanceList.Find(x => !x.gameObject.activeInHierarchy);

		if (fish == null) {
			GameObject newInst = Instantiate(fishPrefab);
			fish = newInst.GetComponent<FishBehaviour>();
			instanceList.Add(fish);
			fish.rect = rect;
		}
		fish.gameObject.SetActive(true);

		fish.transform.position = new Vector3(UnityEngine.Random.Range(rect.x + rect.width, rect.x + rect.width+3), UnityEngine.Random.Range(rect.y, rect.y + rect.height),0);
	}

	private void OnDrawGizmos() {
		Gizmos.DrawLine(new Vector3(rect.x, rect.y, 0), new Vector3(rect.x + rect.width, rect.y, 0));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y + rect.height, 0), new Vector3(rect.x + rect.width, rect.y + rect.height, 0));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y, 0), new Vector3(rect.x, rect.y + rect.height, 0));
		Gizmos.DrawLine(new Vector3(rect.x + rect.width, rect.y, 0), new Vector3(rect.x + rect.width, rect.y + rect.height, 0));
	}

}

public class FishSave {
	public int count;
	public string time;
}
