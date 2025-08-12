using UnityEngine;
using System.Collections;

public class IncomingTopIcons : MonoBehaviour {

    public delegate void Active(float positionX);
    public event Active OnActive;
    public AudioClip eventAudio;

    [HideInInspector] public bool moveToPlayerX;
  
    Vector3 velocity;
    
    void OnEnable() {
        if (Player.Jack.PlayerController.Instance == null) Destroy(gameObject);
        
    }

    public void SetActiveAnim() {
        GetComponent<Animator>().SetTrigger("active");
    }

    void Update() {
        velocity.x = RunnerController.RunSpeed;
        if (moveToPlayerX) {
            velocity.x += Player.Jack.PlayerController.Instance.transform.position.x - transform.position.x;
        }
        transform.position += velocity * Time.deltaTime;
    }
    
	public void EndAnimEvent() {
        if (OnActive != null) OnActive(transform.position.x);
        Destroy(gameObject);
    }

    public void AudioEvent() {
        if(eventAudio != null)
            AudioManager.PlayEffect(eventAudio, AudioMixerTypes.runnerEffect);
    }
}
