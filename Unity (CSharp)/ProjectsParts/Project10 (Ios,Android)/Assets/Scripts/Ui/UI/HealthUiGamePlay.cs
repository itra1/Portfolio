using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUiGamePlay : MonoBehaviour {

	public List<HealthPanel> healthList;

	[System.Serializable]
	public struct HealthPanel {
		public HealthType type;
		public GameObject panel;
	}

	private void OnEnable() {
		healthList.ForEach(x=>x.panel.SetActive(x.type == GameManager.activeLevelData.healthType));
	}
	
}
