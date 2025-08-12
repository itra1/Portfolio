using UnityEngine;
using System.Collections;

public class SplashCamera : MonoBehaviour {

    public SplashController splash;

    bool start;

    void OnPostRender()
    {
        if (start) return;
        StartCoroutine(WaitAndStart());
        start = true;
    }

    IEnumerator WaitAndStart() {
        yield return new WaitForSeconds(0.5f);
        splash.StartAnim();
    }
}
