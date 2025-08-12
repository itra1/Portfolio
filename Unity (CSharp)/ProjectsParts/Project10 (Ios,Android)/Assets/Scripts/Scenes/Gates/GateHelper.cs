using UnityEngine;

public class GateHelper : MonoBehaviour {

    public gateController gateCom;

    public delegate void CheckPlayer();
    public CheckPlayer OnCheck;

    void OnTriggerEnter2D(Collider2D obj) {

        if(obj.tag == "Player") {
            //gateCom.OpenGate();
            if (OnCheck != null) OnCheck();
        }

    }
}
