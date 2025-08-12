using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AchiveView))]
public class AchiveViewEditor: Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Open")) {
			((AchiveView)target).achiveList[0].gameObject.SetActive(true);
		}

	}
}

#endif

[System.Serializable]
public class AchiveView : EventBehaviour {

	public List<AchiveDecor> achiveList = new List<AchiveDecor>();

	private int? readyAchiveShow;
	
	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.OnAddDecor))]
	public void ChengeAchive(ExEvent.PlayerEvents.OnAddDecor decor) {
		OnAddAchive(decor.decorNum);
	}

	private void Start() {
		achiveList.ForEach(x=>x.gameObject.SetActive(false));
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.OnLoad))]
	void OnLoadPlayer(ExEvent.PlayerEvents.OnLoad onLoad) {
		Init();
	}

	private void Init() {
		for (int i = 0; i < achiveList.Count; i++) {
			achiveList[i].gameObject.SetActive(PlayerManager.Instance.achives.openAchives.Contains(i));
		}
	}

	public void OnAddAchive(int achiveNum) {
		readyAchiveShow = achiveNum;
		Show();
	}

	public void Show() {
		if (readyAchiveShow == null) return;

		var showElem = achiveList[(int)readyAchiveShow];
		showElem.gameObject.SetActive(true);
		showElem.FirstShow();
		readyAchiveShow = null;

	}

}
