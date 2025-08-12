using UnityEngine;
using System.Collections;

public class CristalButton : MonoBehaviour {
    
    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
            GameObject.Find("SceneController").GetComponent<SceneMainCintroller>().GoRun();
    }
}
