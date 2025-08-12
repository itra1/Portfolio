using UnityEngine;
using Spine.Unity;

public class GhostController : MonoBehaviour {

  public SkeletonAnimation skeletonAnimation;                     // Ссылка на скелетную анимацию
  Vector3 velocity;
  public int PlayerDamage;
  public GameObject incomingPref;
  public LayerMask playerLayer;                                   // Слой игрока
  public float playerRadius;                                      // Радиус определения игрока

  [SerializeField]
  PlayerDamage playerDamag;                                       // Структура, для атаки по игроку
  float lastDamage;                                               // Время последней атаки
  public LayerMask enemyLayer;                                    // Слой с врагами

  string currentAnimation;                                        // Текущая анимация
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string idleAnim = "";                                    // Анимация дамага сзади
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string attackAnim = "";                                  // Анимация дамага сзади
  
  AudioSource audioComp;                                          // Компонент аудио
  public AudioClip[] enemyIdleAudio;                              // Звуки при беге
  public FloatSpan audioTime;                                     // Время между звучанием
  float nextIdleAudio;                                            // Время следуюзего звучания
  public AudioClip[] enemyDamageAudio;                            // Звуки при беге
  
  void OnEnable() {

    GameObject inct = Instantiate(incomingPref, new Vector3(CameraController.displayDiff.rightDif(1) - 1, transform.position.y, transform.position.z),Quaternion.identity) as GameObject;
    inct.GetComponent<IncomingIcon>().OnComplited = GhostStart;
    velocity.x = RunnerController.RunSpeed;
    audioComp = GetComponent<AudioSource>();
  }

  void Update() {
    SetAnimation(idleAnim, true);
    
    transform.position += velocity * Time.deltaTime;

    //Аудио
    if(nextIdleAudio <= Time.time && transform.position.x <= CameraController.displayDiff.rightDif(1)) {
      PlayIdleAudio();
      nextIdleAudio = Time.time + Random.Range(audioTime.min, audioTime.max);
    }

    CheckPlayer();

    if(transform.position.x <= CameraController.displayDiff.leftDif(2)) gameObject.SetActive(false);
  }

  public void GhostStart() {
    velocity.x = RunnerController.RunSpeed - 4;
  }


  // Воспроизведение стандартного аудио бега
  void PlayIdleAudio() {
    if(enemyIdleAudio.Length > 0) {
      audioComp.PlayOneShot(enemyIdleAudio[Random.Range(0, enemyIdleAudio.Length)], 1);
    }
  }

  void PlayDamageAudio() {
    if(enemyDamageAudio.Length > 0) {
      audioComp.PlayOneShot(enemyDamageAudio[Random.Range(0, enemyDamageAudio.Length)], 1);
    }
  }

  bool CheckPlayer() {

    bool isPlayer = Physics2D.CircleCast(transform.position, playerRadius, Vector2.zero, playerLayer);
    if(isPlayer && lastDamage <= Time.time) {
      PlayDamageAudio();
      Player.Jack.PlayerController.Instance.ThisDamage(WeaponTypes.none, Player.Jack.DamagType.live, playerDamag.damagePower, transform.position);
      AddAnimation(2, attackAnim, false, 0);
      lastDamage = Time.time + Random.Range(playerDamag.damageTime.min, playerDamag.damageTime.max);
      return true;
    }
    return false;
  }

  #region Animation
  
  public void SetAnimation(string anim, bool loop) {
    if(!gameObject) return; // Вероятно исправляет ошибку вылета

    if(currentAnimation != anim) {
      skeletonAnimation.state.SetAnimation(0, anim, loop);
      currentAnimation = anim;
    }
  }
  
  public void ResetAnimation() {
    skeletonAnimation.Initialize(true);
    skeletonAnimation.state.Event += AnimEvent;
    currentAnimation = null;
  }
  
  public void AddAnimation(int index, string animName, bool loop, float delay) {
    skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
  }
  
  #endregion

  void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
    bool isPlayer = Physics2D.CircleCast(transform.position, playerRadius,Vector2.up, playerLayer);
    if(isPlayer) {
      PlayDamageAudio();

      Player.Jack.PlayerController.Instance.ThisDamage(WeaponTypes.none, Player.Jack.DamagType.live, playerDamag.damagePower, transform.position);
    }
  }
}
