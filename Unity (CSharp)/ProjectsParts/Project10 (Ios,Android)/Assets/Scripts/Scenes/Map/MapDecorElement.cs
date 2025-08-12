using UnityEngine;
using Spine.Unity;

/// <summary>
/// Контроллер элемента на сцене
/// </summary>
public class MapDecorElement : MonoBehaviour {

  public SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию

  public FloatSpan rateRareAnim;

  [SpineAnimation(dataField: "skeletonAnimation")]
  public string idleCateAnim = "";                  // Анимация дамага сзади
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string idleRareAnim = "";                 // Анимация дамага спереди

  float nextTimeRareAnim;
  bool isTouch;

  void Start() {
		MapGamePlay.OnTapDrag += Drag;
    nextTimeRareAnim = Time.time + Random.Range(rateRareAnim.min, rateRareAnim.max);

    // Проверка на наличие анимации покоя
    idleCateAnim = CheckAnim(idleCateAnim);

    if(idleCateAnim != null && idleCateAnim != "")
      SetAnimation(idleCateAnim, true);

    skeletonAnimation.state.Complete += AnimCompl;
  }

  void OnDestroy() {
		MapGamePlay.OnTapDrag -= Drag;
  }

  void Drag(Vector2 deltaTouch) {
    if(deltaTouch.x != 0)
      isTouch = false;
  }

  void Update() {
    if(nextTimeRareAnim < Time.time) {
      SetAnimation(idleRareAnim, false);
      nextTimeRareAnim = Time.time + Random.Range(rateRareAnim.min, rateRareAnim.max);
    }
  }

  #region Animation
  /// <summary>
  /// Применение анимации
  /// </summary>
  /// <param name="anim"></param>
  /// <param name="loop"></param>
  public void SetAnimation(string anim, bool loop) {
    if(!gameObject) return; // Вероятно исправляет ошибку вылета

    skeletonAnimation.state.SetAnimation(0, anim, loop);
  }
  /// <summary>
  /// Сброс анимации
  /// </summary>
  public void ResetAnimation() {
    skeletonAnimation.Initialize(true);
    skeletonAnimation.state.Complete += AnimCompl;
  }
  /// <summary>
  /// Добавление анимации
  /// </summary>
  /// <param name="index"></param>
  /// <param name="animName"></param>
  /// <param name="loop"></param>
  /// <param name="delay"></param>
  public void AddAnimation(int index, string animName, bool loop, float delay) {
    skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
  }
  /// <summary>
  /// Установка скорости воспроизведения анимации
  /// </summary>
  /// <param name="speed"></param>
  public void SpeedAnimation(float speed) {
    skeletonAnimation.timeScale = speed;
  }
  #endregion

  void AnimCompl(Spine.AnimationState state, int trackIndex, int loopCount) {
    if(state.GetCurrent(trackIndex).ToString() == idleRareAnim) {
      if(idleCateAnim != null && idleCateAnim != "") {
        SetAnimation(idleCateAnim, true);
      }
    }
  }

  string CheckAnim(string animClip) {

    Spine.ExposedList<Spine.Animation> anim = skeletonAnimation.Skeleton.data.Animations;
    bool yes = false;
    foreach(Spine.Animation an in anim) {
      if(an.Name == animClip) yes = true;
    }

    if(yes)
      return animClip;
    else
      return "";
  }

  void OnMouseDown() {
    isTouch = true;
  }

  void OnMouseUp() {
    if(!isTouch) return;
    SetAnimation(idleRareAnim, false);
    nextTimeRareAnim = Time.time + Random.Range(rateRareAnim.min, rateRareAnim.max);
    if(GameObject.FindObjectOfType<MapController>().TapClick(gameObject.GetInstanceID(), transform.position)) {
      SetAnimation(idleRareAnim, false);
      nextTimeRareAnim = Time.time + Random.Range(rateRareAnim.min, rateRareAnim.max);
    }
  }

  #region Editor
  public LayerMask groundMask;
  public void PositingGround() {

    Ray ray = new Ray(transform.position+new Vector3(0,0,-2),new Vector3(0,0,10));
    RaycastHit hit;
    Physics.Raycast(ray, out hit, 7, groundMask);
    transform.position = hit.point;
  }

  #endregion

}
