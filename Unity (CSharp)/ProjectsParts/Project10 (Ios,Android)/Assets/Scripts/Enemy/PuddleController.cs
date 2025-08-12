using UnityEngine;
using System.Collections;

public class PuddleController : MonoBehaviour {

	void OnTriggerEnter(Collider oth) {

        if(LayerMask.LayerToName(oth.gameObject.layer) == "Player") {
            oth.GetComponent<Player.Jack.PlayerMove>().isDrag = true;
        }
    }

    void OnTriggerExit(Collider oth) {

        if(LayerMask.LayerToName(oth.gameObject.layer) == "Player") {
            oth.GetComponent<Player.Jack.PlayerMove>().isDrag = false;
        }
    }
}
