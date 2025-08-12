using UnityEngine;
using System.Collections;

public class MainObjectsController : MonoBehaviour {

    public SceneMainCintroller scene;
    public AdController adController;

    public void EndFly()
    {
        scene.JackStart();
    }

    public void Ready() {
        GetComponent<Animator>().SetTrigger("ready");
        StartCoroutine(waitReady());
        adController.GetBanner();

    }
    
    IEnumerator waitReady() {
        yield return new WaitForSeconds(7);
        GetComponent<Animator>().SetTrigger("readyLong");
    }
}
