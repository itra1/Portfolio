using UnityEngine;
using Spine.Unity;

[System.Serializable]
public struct SpiderSpeed {
  public float SpeedMin;
  public float SpeedMax;
}
/// <summary>
/// Контроллер паука
/// </summary>
public class Spider : MonoBehaviour {

	public Rigidbody2D rb;
	public EventAction<Vector3> OnDead;                     // Событие смерти паука
  public SpiderSpeed Speed;                               // Скорость движения
  private Vector3 velocity;                               // Рассчет смещения
  public float gravity;                                   // Сила гравитации
  public LayerMask groundMask;                            // Слой с поверхностью
  private bool isGround;                                  // Флаг определения поверхности
  public GameObject shadowObject;                         // Объект тени
  private GameObject shadowInstance;                      // Экземпляр тени
  
  public SkeletonAnimation skeletonAnimation;             // Ссылка на скелетную анимацию
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string idleAnim = "";                            // Анмиация бега
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string deadAnim = "";                            // Анимация сметри

  private AudioSource audioComp;
  private float nextIdlePlay;
  public AudioClip[] idleAudio;
  
  [SerializeField]
  private AudioClip deadClip;
  [SerializeField]
  private int coinsCount;

  private bool isEnable;

  void OnEnable() {
    RunnerController.OnChangeRunnerPhase += ChangePhase;
    isEnable = true;

    GetComponent<BoxCollider2D>().enabled = true;
    velocity.x = RunnerController.RunSpeed - Random.Range(Speed.SpeedMin, Speed.SpeedMax);

		if (shadowInstance == null) {

			shadowInstance = Instantiate(shadowObject, transform.position, Quaternion.identity) as GameObject;
			shadowInstance.GetComponent<ShadowBehaviour>().matherObject = gameObject.transform;
			shadowInstance.GetComponent<ShadowBehaviour>().diff = 0f;
			shadowInstance.transform.parent = transform;
		}
    InitAudio();
	}

	private void Update() {
		rb.velocity = new Vector2(velocity.x, rb.velocity.y);
	}

	void OnDisable() {
    RunnerController.OnChangeRunnerPhase -= ChangePhase;
		if (shadowInstance)
			shadowInstance.SetActive(false);
	}

  public void ChangePhase(RunnerPhase newPhase) {
    if(newPhase == RunnerPhase.dead) velocity.x = 2f;
  }
	

  void LateUpdate() {
    if(!isEnable)
      return;
    isEnable = false;
    skeletonAnimation.Initialize(true);
    skeletonAnimation.state.SetAnimation(0, idleAnim, true);
  }
  
  /// <summary>
  /// Убийство паука
  /// </summary>
  public void Dead() {
    skeletonAnimation.state.SetAnimation(0, deadAnim, false);
    AudioManager.PlayEffect(deadClip, AudioMixerTypes.runnerEffect);
    
    GetComponent<BoxCollider2D>().enabled = false;
    Helpers.Invoke(this, ()=> {
      gameObject.SetActive(false);
    }, 0.2f);
  }

	public void PlayerDamage() {
		if (OnDead != null)
			OnDead(transform.position);
		Dead();
	}

  public void ShadowFixed(bool flag) {
    if(shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().fixedsize = flag;
  }

  public void ShadowSetDiff(Vector3 newPos, Vector3 newScale) {
    if(shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().SetDiff(newPos, newScale);
  }

  public void ShadowSetDeff() {
    if(shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().SetDeff();
  }

  #region Audio

  void InitAudio() {
    audioComp = GetComponent<AudioSource>();
  }

  public void PlayIdleAudio() {
    if(idleAudio.Length > 0) {
      audioComp.PlayOneShot(idleAudio[Random.Range(0, idleAudio.Length)], 1);
    }
  }
  #endregion
}
