using UnityEngine;

public class ManagersLoader : MonoBehaviour {

	public GameObject managers;
	public GameObject graphicManager;

	private void Awake() {
		if (FindObjectOfType(typeof(GameManager)) != null) return;
		Instantiate(managers);
		Instantiate(graphicManager);
	}
	
}
