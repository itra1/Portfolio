using UnityEngine;
using Spine.Unity;

/// <summary>
/// Оружие
/// </summary>
public class Weapon : MonoBehaviour {
  
  public GameObject graphic;

  public float deffAngle;   // Стандартный угол наклона
  public float diffAngle;

  public Transform shootPoint;

  bool lastIsHide;
  bool lastIsFirst;

  bool init;

  private void Start() {
    ChangeAngle(false);
    init = true;
    skeletonAnimation.GetComponent<MeshRenderer>().enabled = false;
    SubscribeAnimEvents();
    IdleAnimPlay();
    ResetAnimation();
		GetComponent<BoneFollowerPlus>().diffAngle = deffAngle;
	}

  private void LateUpdate() {
    if(init) {
      init = false;
      skeletonAnimation.GetComponent<MeshRenderer>().enabled = true;
    }
  }

  public void ChangeAngle(bool isReady) {
		return;
		GetComponent<BoneFollowerPlus>().diffAngle = (isReady ? deffAngle : (graphic.transform.localScale.x > 0 ? deffAngle + diffAngle : deffAngle - diffAngle));
  }

  public void SetHidden(bool isHide, bool isFirst = false) {
    //ResetAnimation();
    lastIsHide = isHide;
    lastIsFirst = isFirst;
    skeletonAnimation.skeleton.Slots.ForEach(x => x.a = (isHide ? (isFirst ? 0.5f : 0) : 1));
  }

  #region Анимация

  public SkeletonAnimation skeletonAnimation;                   // Ссылка на спайн анимацию
  private string currentAnimation;                              // Текущая анимация

  [SpineBone(dataField: "skeletonAnimation")]
  //public string shootBone;
  //Spine.Bone shootBoneGun;

  [SpineAnimation(dataField: "skeletonAnimation")]
  public string shootAnimation = "";                            // Анимация бега
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string staticAnimation = "";                           // Анимация бега

  /// <summary>
  /// Запуск анимации выстрела
  /// </summary>
  public void ShootAnimPlay() {
    SetAnimation(shootAnimation, false);
  }

  /// <summary>
  /// Анимация простоя
  /// </summary>
  public void IdleAnimPlay() {
    SetAnimation(staticAnimation, true);
  }

  /// <summary>
  /// Установка основной анимации
  /// </summary>
  /// <param name="anim">название анимации</param>
  /// <param name="loop"></param>
  public void SetAnimation(string anim, bool loop, bool noEndEvent = false) {
    if(!gameObject) return;

    // Устанавливаем анимацию
    if(currentAnimation != anim || loop == false) {
      skeletonAnimation.state.SetAnimation(0, anim, loop);
      currentAnimation = anim;
    }
  }

  /// <summary>
  /// Сброс настроек анимации
  /// </summary>
  public void ResetAnimation() {
    skeletonAnimation.Initialize(true);
    SubscribeAnimEvents();
    currentAnimation = null;
    SetHidden(lastIsHide, lastIsFirst);
  }

  /// <summary>
  /// Подписваемся на событие анимации
  /// </summary>
  protected void SubscribeAnimEvents() {
    skeletonAnimation.state.Start += AnimStart;
    skeletonAnimation.state.Event += AnimEvent;
    skeletonAnimation.state.Complete += AnimComplete;
    skeletonAnimation.state.End += AnimEnd;
    skeletonAnimation.state.Dispose += AnimDispose;
    skeletonAnimation.state.Interrupt += AnimInterrupt;
  }

  /// <summary>
  /// Наложение анимации
  /// </summary>
  /// <param name="index">Номар слоя</param>
  /// <param name="animName">Название анимации</param>
  /// <param name="loop">Зациклено</param>
  /// <param name="delay">Задержка запуска</param>
  public void AddAnimation(int index, string animName, bool loop, float delay) {
    skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
  }

  /// <summary>
  /// Привызяка к событию анимации
  /// </summary>
  /// <param name="state">Анимация</param>
  /// <param name="trackIndex">Слой анимации</param>
  /// <param name="e">Событие</param>
  void AnimEvent(Spine.TrackEntry trackEntry, Spine.Event e) {
	
	if(e.Data.name == "Attack") {
		}
	
	}

  public Actione OnAttackEnd;

  /// <summary>
  /// Привязка к окончанию анимации
  /// </summary>
  /// <param name="state">Анимация</param>
  /// <param name="trackIndex">Слой анимации</param>
  void AnimEnd(Spine.TrackEntry trackEntry) {
    if(trackEntry.ToString() == shootAnimation) {
      if(OnAttackEnd != null) OnAttackEnd();
    }

  }

  /// <summary>
  /// Событие начала оанимации
  /// </summary>
  /// <param name="state">Анимация</param>
  /// <param name="trackIndex">Слой анимации</param>
  void AnimStart(Spine.TrackEntry trackEntry) {
    if(trackEntry.ToString() == shootAnimation) {
      ChangeAngle(true);
			
			GameObject damageHit = PoolerManager.GetPooledObject("PistolPaf");
			damageHit.transform.localScale = graphic.transform.localScale;
			damageHit.transform.position = (Vector2)shootPoint.position;
			damageHit.SetActive(true);

			try {
				GetComponent<AudioSource>().Play();
			} catch { }
		}
  }

  /// <summary>
  /// Событие выполнения анимации при зацикленном воспроизведении
  /// </summary>
  /// <param name="state">Анимация</param>
  /// <param name="trackIndex">Слой анимации</param>
  /// <param name="loopCount">Цикл</param>
  void AnimComplete(Spine.TrackEntry trackEntry) {
    if(trackEntry.ToString() == shootAnimation) {
      IdleAnimPlay();
      ChangeAngle(false);
    }
  }
  void AnimDispose(Spine.TrackEntry trackEntry) { }
  void AnimInterrupt(Spine.TrackEntry trackEntry) { }

  #endregion

}
