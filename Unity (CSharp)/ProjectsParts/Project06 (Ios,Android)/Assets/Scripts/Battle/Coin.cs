using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

  public float startSpeedY;
  public float gravity;

  Vector3 velocity =Vector3.zero;

  public GameObject silverSprite;
  public GameObject goldSprite;

  public bool isGold;
  public int nomination = 1;
  public float deltaPosition;

  void OnEnable() {
    //Debug.Log(deltaPosition);
    velocity.x = deltaPosition*3;
    velocity.y = startSpeedY;
    silverSprite.SetActive(!isGold);
    goldSprite.SetActive(isGold);
    if(!isGold) {
      silverSprite.transform.localScale = Vector2.one * (1 + ((nomination - 1) * 0.05f));
    }
  }


  void Update() {
    velocity.y -= gravity * Time.deltaTime;
    transform.position += velocity * Time.deltaTime;
    if(transform.position.y < -10) gameObject.SetActive(false);
  }

}
