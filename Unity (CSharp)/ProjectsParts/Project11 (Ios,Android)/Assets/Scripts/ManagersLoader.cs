using UnityEngine;

public class ManagersLoader : MonoBehaviour {

	public GameObject managers;
  public GameObject canvas;

  private void Awake() {
		if (FindObjectOfType(typeof(GameManager)) != null) return;
		Instantiate(managers);
    Instantiate(canvas);
  }
	
}
