using UnityEngine;
using ExEvent;

public class LoadUi : EventBehaviour {

	public GameObject logo;
	public GameObject loadPanel;

	private void Start() {
		logo.SetActive(true);
		loadPanel.SetActive(true);
	}

	//[ExEventHandler(typeof(ExEvent.LoadEvents.StartLoadProgress))]
	//private void StartLoad(LoadEvents.StartLoadProgress startProgress) {
	//	loadPanel.SetActive(true);
	//}

}
