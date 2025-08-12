using UnityEngine;

public class IncomingIcon : MonoBehaviour {

  public EventAction OnComplited;

  public void DestroyHis() {
    if(OnComplited != null) OnComplited();
    Destroy(gameObject);
  }

  public void AudioEvent() {
  }

}
