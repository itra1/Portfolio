using UnityEngine;

public class ManagersLoader : MonoBehaviour {

	public GameObject managers;

	private void Awake() {
    
    if (FindObjectOfType(typeof(GameManager)) != null) return;
		Instantiate(managers);
	}
	
}
