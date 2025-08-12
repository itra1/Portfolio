using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BonusController: MonoBehaviour, IPointerDownHandler {
  public float helth = 0;
  public float softCash = 0;
  public float hardCash = 0;
  //public float patron = 0;
  public SpriteRenderer image;
  Vector3 velocity;
  bool inMove;
  Vector3 targetPanel;
  float targetY;
  float stopYPanel;

  public Animation animComponent;


  void Start() {

    if (softCash != 0) {
      targetPanel = BattleController.Instance.panel.iconSoftCash.position;
    }

    if (hardCash != 0) {
      targetPanel = BattleController.Instance.panel.iconHardCash.position;
    }

    if (helth != 0) {
      Invoke("DestroyHelth", 2f);
      //targetPanel = GameController.Instance.panel.iconHardCash.position;
    }




    stopYPanel = BattleController.Instance.panel.iconHardCash.position.y;

    image.sortingOrder = -(int)(transform.position.y * 100);

    inMove = true;
    //startPosition = transform.position;
    velocity = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(5f, 7f), 0);
    targetY = transform.position.y - 0.3f;
    Invoke("MoveInPanel", 2f);

  }

  void Update() {
    if (moveInPanel && transform.position.y < stopYPanel - 0.1f) {
      transform.position += velocity * 7 * Time.deltaTime;

    }

    if (moveInPanel && transform.position.y > stopYPanel - 0.1f) {

      if (softCash != 0) UserManager.Instance.GetSoftCashBonus((int)softCash);
      if (hardCash != 0) UserManager.Instance.GetHardCashBonus((int)hardCash);
      Destroy(gameObject);

    }
    if (!inMove) return;
    if (transform.position.y < targetY) {
      inMove = false;

      animComponent.Play("onGround");
      return;
    }

    velocity.y -= 30 * Time.deltaTime;
    transform.position += velocity * Time.deltaTime;

  }


  public void DestroyHelth() {
    UserManager.Instance.UseMedBonus(helth);
    Destroy(gameObject);
  }

  public void OnPointerDown(PointerEventData eventData) {

    if (helth != 0) UserManager.Instance.UseMedBonus(helth);
    if (softCash != 0) UserManager.Instance.GetSoftCashBonus((int)softCash);
    if (hardCash != 0) UserManager.Instance.GetHardCashBonus((int)hardCash);
    //if (patron != 0) UserManager.Instance.GetPatronBonus(patron);

    Destroy(gameObject);

  }
  bool moveInPanel = false;
  public void MoveInPanel() {

    velocity = (targetPanel - transform.position).normalized;
    moveInPanel = true;

  }



}
