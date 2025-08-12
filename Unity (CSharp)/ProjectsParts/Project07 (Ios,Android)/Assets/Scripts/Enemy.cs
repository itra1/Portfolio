using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Enemy : ExEvent.EventBehaviour, IPointerDownHandler {

	public EnemyHealthProgressBar liveLine;
  public EnemyAttackProgressBar attackLine;
  public Canvas liveLineCanvas;
  public Transform hitPosition;
	private Animator _animator;

  [SerializeField]
  private SpriteRenderer _spriteBody;

  private BattleController.BattlePhase phase;

  [HideInInspector]
  public Animator animator {
    get {
      if (_animator == null)
        _animator = GetComponent<Animator>();
      return _animator;
    }
  }

	public ParticleSystem iskra;
	float damage;

	public bool isDead { get; private set; }
  
  //количество жизней
  float health = 5;
	float maxHealth;
	float timeNextAttack;
  float timeLastAttack;
  float periodAttack;

  private int sorting = 0;

	void OnEnable () {
    
    liveLine.SetPercent(1);
    isDead = false;

    phase = BattleController.Instance.battlePhase;

  }
  
  public void SetData(EnemyManager.Position enemyPosition, float health, float damage, float timeReload) {
		maxHealth = health;
		this.health = health;
    periodAttack = timeReload;
    transform.position = enemyPosition.position;
    transform.localScale = enemyPosition.scaling;
    _spriteBody.sortingOrder = -(int)(transform.position.y * 100);
		liveLineCanvas.sortingOrder = -(int)(transform.position.y * 100);
    sorting = -(int)(transform.position.y * 100);
    //iskra.GetComponent<MeshRenderer>().sortingOrder = -(int)((transform.position.y * 100) - 1);
    iskra.GetComponent<ParticleSystemRenderer>().sortingOrder = -(int)((transform.position.y * 100) - 1);

    ResetAttackTimer();
    this.damage = damage;

	}

  private void ResetAttackTimer() {
    timeNextAttack = GameTime.time + periodAttack;
    timeLastAttack = GameTime.time;
  }
  private void AddAttackTimer(float rercent = 1) {
    timeLastAttack += periodAttack * rercent;
    if (timeLastAttack > GameTime.time)
      timeLastAttack = GameTime.time;
    timeNextAttack = timeLastAttack + periodAttack;
  }
  
  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattlePhaseChange))]
  private void BattlePhaseChange(ExEvent.BattleEvents.BattlePhaseChange eventData) {
    
    if((eventData.oldPhase & BattleController.BattlePhase.end) != 0 && (eventData.phase & BattleController.BattlePhase.end) == 0) {
      ResetAttackTimer();
    }
    if ((eventData.oldPhase & BattleController.BattlePhase.end) == 0 && (eventData.phase & BattleController.BattlePhase.end) != 0) {
      ResetAttackTimer();
    }

  }

  void Update () {

    if ((BattleController.Instance.battlePhase & BattleController.BattlePhase.end) != 0)
      return;

    if (GameTime.time > timeNextAttack) {
      Atack();
    } else {
      attackLine.SetPercent(((timeNextAttack- GameTime.time)/(timeNextAttack- timeLastAttack)));
    }
	}

	public void OnPointerDown(PointerEventData eventData) {
    //Damage();

    QuestionManager.Instance.AddValueQuest(QuestionManager.Type.click, 1);
    ExEvent.BattleEvents.EnemyClick.Call(this);

  }

	void Atack() {
    ResetAttackTimer();
    //if (AtackEvent != null) AtackEvent(this);
    //Debug.Log(damage);
    UserManager.Instance.Damage(damage);
    animator.SetTrigger("attack");
    DropSpawner.Instance.GreateAttackLabel(sorting, hitPosition.position, transform.localScale);

  }

	/// <summary>
  /// Получение урона
  /// </summary>
  /// <param name="damageValue"></param>
	public void Damage(float damageValue) {

    if (isDead) return;

    iskra.transform.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), (1 + Random.Range(-0.5f,0.5f)), 0);

    iskra.Play();
    if(!BattleController.Instance.isBoss)
      AddAttackTimer(0.3f);

    //damageAudios[Random.Range(0, damageAudios.Count)].Play();

    DarkTonic.MasterAudio.MasterAudio.PlaySound("EnemyHit");

    health -= damageValue;
    if (OnDamageEvent != null)
      OnDamageEvent(this);
    DropSpawner.Instance.GreateHinCount(damageValue, sorting, hitPosition.position, transform.localScale);

    if (health <= 0)
      health = 0;

    liveLine.SetPercent(health / maxHealth);




    if (health <= 0) {
      Dead();
      return;
    }

    animator.SetTrigger("damage");
    
  }
  private void Dead() {
		if (StartDead != null) StartDead(this);
		animator.SetTrigger("dead");
    isDead = true;
  }

  public void DeadComplete() {
    gameObject.SetActive(false);
    if (DeadEvent != null) DeadEvent(this);
  }

  public void SetBodyGraphic(int spriteIndex)
  {
    _spriteBody.sprite = EnemyManager.Instance.GetSprite(spriteIndex);
  }

	public static event System.Action<Enemy> DeadEvent;
  public static event System.Action<Enemy> OnDamageEvent;
  public System.Action<Enemy> StartDead;
  //public static event System.Action<Enemy> AtackEvent;


  public List<AudioClipData> damageAudios;
}
