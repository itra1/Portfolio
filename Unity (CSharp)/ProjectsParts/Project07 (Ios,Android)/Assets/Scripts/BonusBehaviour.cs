using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BonusBehaviour: MonoBehaviour, IPointerDownHandler {

  [HideInInspector]
  public float count;
  public SpriteRenderer image;

  private Vector3 velocity;
  private bool inMove;
  protected Vector3 targetPanel;
  private float targetY;
  private float stopYPanel;
  public Animation animComponent;

  protected virtual void Start() {

    stopYPanel = BattleController.Instance.panel.iconHardCash.position.y;

    image.sortingLayerName = "Enemy";
    image.sortingOrder = -(int)(transform.position.y * 100);

    inMove = true;
    //startPosition = transform.position;
    velocity = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(5f, 7f), 0);
    targetY = transform.position.y - 0.3f;

    StartCoroutine(WaitFunc(MoveInPanel, 2f));
  }

  private void Update() {
    if (moveInPanel && transform.position.y < stopYPanel - 0.1f) {
      transform.position += (targetPanel - transform.position).normalized * 10 * Time.deltaTime;

    }

    if (moveInPanel && transform.position.y > stopYPanel - 0.1f) {

      GetBonus();
      Destroy(gameObject);

    }
    if (!inMove) return;
    if (transform.position.y < targetY) {
      inMove = false;

      animComponent.Play("onGround");
      return;
    }

    transform.position += velocity * Time.deltaTime;
    velocity.y -= 30 * Time.deltaTime;

  }

  bool moveInPanel = false;
  public void MoveInPanel() {

    moveInPanel = true;
    velocity = (targetPanel - transform.position).normalized;

  }

  public virtual void OnPointerDown(PointerEventData eventData) {
    if (moveInPanel)
      return;
    StopAllCoroutines();
    MoveInPanel();
  }

  protected virtual void GetBonus() {
    StopAllCoroutines();
    //if (getAudio.Count > 0)
    //  getAudio[Random.Range(0, getAudio.Count)].Play();
  }

  protected IEnumerator WaitFunc(System.Action function, float wait) {

    yield return new WaitForSeconds(wait);
    function();
  }


  public List<AudioClipData> getAudio;

}
