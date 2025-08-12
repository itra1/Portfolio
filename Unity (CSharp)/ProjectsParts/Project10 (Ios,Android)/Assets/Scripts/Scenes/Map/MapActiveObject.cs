using UnityEngine;
using System.Collections;
using Spine.Unity;

public class MapActiveObject : MonoBehaviour {

  public MapBonusGenerate bonus;
  public TimesBonuses timeBonus;
  public GameObject spriteAnim;
  public AudioClip tapClip;

  public bool isGull;
  bool muve;
  bool isClick;

  public Animator spriteTapAnimator;

  public enum ActiveObjectPhase { start, idol, end }
  public ActiveObjectPhase phase;

  float timeStart;

  public SkeletonAnimation skeletonAnimation;
  public SpriteRenderer sprite;
  public GameObject graphic;

  float timeTapWait;

  [SerializeField]
  FloatSpan timeAnimRareRande;
  float nextAnimRare;

  string currentAnimation;                                        // Текущая анимация
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string idolAnim = "";                                    // Анимация дефалтная
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string idolRareAnim = "";                                // Случайная анимация
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string tapAnim = "";                                     // Анимация тапа
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string vanishAnim = "";                                  // Анимация сокрытия
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string appearanceAnim = "";                              // Анимация появления

  bool isEnable;

  void OnEnable() {
		MapGamePlay.OnTapDrag += Drag;
    isEnable = true;
    nextAnimRare = Time.time + Random.Range(timeAnimRareRande.min, timeAnimRareRande.max);
    phase = ActiveObjectPhase.start;
    timeStart = Time.time;
  }

  void OnDisable() {
		MapGamePlay.OnTapDrag -= Drag;
  }

  void Drag(Vector2 deltaTouch) {
    isClick = false;
  }

  void Update() {

    if(isGull && muve) {
      transform.position += new Vector3(-1, 2, 0) * Time.deltaTime;
      if(transform.position.y > 5) Destroy(gameObject);
    }

    if(phase == ActiveObjectPhase.end) return;
    if(timeStart + 1f <= Time.time && idolAnim != null) SetAnimation(idolAnim, true);

    if(phase == ActiveObjectPhase.start && timeStart + 1 < Time.time) phase = ActiveObjectPhase.idol;

    if(phase == ActiveObjectPhase.idol) {
      SetAnimation(idolAnim, true);

      if(nextAnimRare <= Time.time) {
        AddAnimation(1, idolRareAnim, false, 0);
        nextAnimRare = Time.time + Random.Range(timeAnimRareRande.min, timeAnimRareRande.max);
      }
    }
  }

  void LateUpdate() {
    if(isEnable) {
      isEnable = false;

      if(appearanceAnim != "")
        SetAnimation(appearanceAnim, true);
      else
        SetAnimation(idolAnim, true);
    }
  }

  void OnMouseUp() {
    if(!isClick) return;

    if(phase != ActiveObjectPhase.idol) return;

    if(tapClip != null)
      AudioManager.PlayEffect(tapClip, AudioMixerTypes.mapEffect);

    if(timeTapWait <= Time.time) {

      if(spriteTapAnimator != null)
        spriteTapAnimator.SetTrigger("tap");

      AddAnimation(2, tapAnim, false, 0);
      timeTapWait = Time.time + 0.5f;
    }
    if(bonus.coinsCount >= 0) {

      if(timeBonus == TimesBonuses.min15) {
        GenerateCoins(1);
        bonus.coinsCount -= 1;
      } else {
        GenerateCoins(bonus.coinsCount);
        bonus.coinsCount = 0;
      }
      GameObject.FindObjectOfType<MapController>().TapObject(timeBonus, bonus);
    }
    nextAnimRare = Time.time + Random.Range(timeAnimRareRande.min, timeAnimRareRande.max);

  }

  void OnMouseDown() {
    isClick = true;

    
  }

  public void SetBonus(TimesBonuses newTimeBonus, MapBonusGenerate newBonus) {
    bonus = newBonus;
    timeBonus = newTimeBonus;
  }

  public void PositingObject(ObjectPosition newPos) {
    graphic.transform.localScale = new Vector3(graphic.transform.localScale.x * (newPos.mirrowX ? -1 : 1), graphic.transform.localScale.y, graphic.transform.localScale.z);
    graphic.transform.localPosition = new Vector3(graphic.transform.localRotation.x, newPos.localDiffY, graphic.transform.localRotation.z);
    if(skeletonAnimation != null)
      skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = newPos.order;
    else if(sprite != null)
      sprite.sortingOrder = newPos.order;
  }

  void GenerateCoins(int coinsCount) {
    StartCoroutine(generateCoins(coinsCount));
  }

  IEnumerator generateCoins(int needNom) {

    MapController map = GameObject.FindObjectOfType<MapController>();

    int nomin = 0;

    while(needNom > 0) {

      if(needNom > 50) {
        nomin = 50;
      } else if(needNom > 10) {
        nomin = 10;
      } else if(needNom > 5) {
        nomin = 5;
      } else if(needNom > 2) {
        nomin = 2;
      } else if(needNom > 0) {
        nomin = 1;
      }
      needNom = needNom - nomin;
      map.GenerateCoins(transform.position, nomin);
      yield return new WaitForSeconds(0.1f);
    }


    if(spriteTapAnimator != null)
      spriteTapAnimator.SetTrigger("close");

    CheckObj();

  }

  [SerializeField]
  AudioClip muveClip;

  void CheckObj() {
    if(bonus.coinsCount <= 0) {
      phase = ActiveObjectPhase.end;
      if(muveClip != null)
        AudioManager.PlayEffect(muveClip, AudioMixerTypes.mapEffect);
      //ResetAnimation();
      if(!isGull) {
        SetAnimation(vanishAnim, false);

        Destroy(gameObject, 2f);
      } else {
        muve = true;

        spriteAnim.SetActive(true);
        skeletonAnimation.gameObject.SetActive(false);
      }
    }
  }

  #region Animation
  /* ***************************
   * Применение анимации
   * ***************************/
  public void SetAnimation(string anim, bool loop) {
    if(!gameObject) return; // Вероятно исправляет ошибку вылета

    if(skeletonAnimation == null || anim == "") return;

    if(currentAnimation != anim) {
      skeletonAnimation.state.SetAnimation(0, anim, loop);
      currentAnimation = anim;
    }
  }


  /* ***************************
   * Резет анимации
   * ***************************/
  public void ResetAnimation() {
    if(skeletonAnimation == null) return;
    skeletonAnimation.Initialize(true);
    currentAnimation = null;
  }


  /* ***************************
   * Добавленная анимация
   * ***************************/
  public void AddAnimation(int index, string animName, bool loop, float delay) {

    if(skeletonAnimation == null || animName == "") return;

    skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
  }

  /* ***************************
   * Установка скорости
   * ***************************/
  public void SpeedAnimation(float speed) {
    skeletonAnimation.timeScale = speed;
  }
  #endregion

  #region Editor
  /// <summary>
  /// Сохраняем позицию элемента в массив MapController.objectsParametrs
  /// </summary>
  /// <param name="arrayElem">Элемент массива</param>
  public void AddPositionToArray(int arrayElem) {

    ObjectPosition newPosition = new ObjectPosition();
    newPosition.position = transform.position;
    newPosition.mirrowX = graphic.transform.localScale.x < 0 ? true : false;
    newPosition.localDiffY = graphic.transform.localPosition.y;
    if(skeletonAnimation != null)
      newPosition.order = skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder;
    else if(sprite != null)
      sprite.sortingOrder = sprite.sortingOrder;

    MapController map = GameObject.FindObjectOfType<MapController>();
    ObjectPosition[] tmp = new ObjectPosition[map.objectsParametrs[arrayElem].position.Length+1];

    for(int i = 0; i < map.objectsParametrs[arrayElem].position.Length; i++)
      tmp[i] = map.objectsParametrs[arrayElem].position[i];

    tmp[tmp.Length - 1] = newPosition;
    map.objectsParametrs[arrayElem].position = tmp;
  }

  public LayerMask groundMask;
  public void PositingGround() {

    Ray ray = new Ray(transform.position+new Vector3(0,0,-2),transform.forward);
    RaycastHit hit;
    Physics.Raycast(ray, out hit, 7, groundMask);
    Debug.Log(hit.point);
    transform.position = hit.point;
  }

  #endregion
}
