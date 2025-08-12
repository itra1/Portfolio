using UnityEngine;

public class Destroys : MonoBehaviour {

	void OnTriggerExit2D(Collider2D oth) {
    if(LayerMask.LayerToName(oth.gameObject.layer) != "Player")
      oth.gameObject.SetActive(false);
  }
}
