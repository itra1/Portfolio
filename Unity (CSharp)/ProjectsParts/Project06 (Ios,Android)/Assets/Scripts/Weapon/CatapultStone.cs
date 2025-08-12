using UnityEngine;
using System.Collections;

/// <summary>
/// Камень из катапульты
/// </summary>
public class CatapultStone : ExEvent.EventBehaviour {
	
  public GameObject graphic;

  [SerializeField]
  private     float       playerDamage;                   //Повреждения наносимые игроку
  [SerializeField]
  private     float       enemyDamage;                    // Повреждения наносимые врагу при отражении
  private     bool        _isRepuls;                      // Флаг отражения
  private     Vector3     velocity;
  public      float       gravity;
  private     float       angle;
  public      float       rotationSpeed;
  float       groundLevel;

  Mathematic mat = new Mathematic();
  public float speed = 15;

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattlePhaseChange))]
	public void PhaseChange(ExEvent.BattleEvents.BattlePhaseChange phase) {
		if (phase.phase == BattlePhase.start) {
			gameObject.SetActive(false);
		}
	}

	void OnEnable() {
    _isRepuls = false;
    //velocity = ( CameraController.middleTopPointWorldX - transform.position ).normalized * acceleration;
    groundLevel = Random.Range(-4.7f , -4.3f);

    float allDistance = (PlayerController.Instance.transform.position - transform.position).magnitude;

    mat.ParabolaCalcCoef(transform.position,
                        new Vector3(PlayerController.Instance.transform.position.x + ((allDistance) / 2), 4, 0),
                        PlayerController.Instance.transform.position
                        );
    Move();
    rotationSpeed = 360 + 360 * (1 - (allDistance/20));
  }

  public void SetAngle(Vector3 angle) {
    graphic.transform.localEulerAngles = new Vector3(angle.x, angle.y, angle.z+30);
    this.angle = graphic.transform.localEulerAngles.z;
  }

  void Update() {
    Move();
    Rotation();
  }

  /// <summary>
  /// Уcтановка параметров снаряда
  /// </summary>
  /// <param name="newRotationSpeed">Скорсть вращения +- 20%</param>
  /// <param name="newPlayerDamage">Урон, наносимый игроку при ударе</param>
  /// <param name="newEnemyDamage">Урон, наносимый врагам при ударе</param>
  public void SetParametrs(float  newPlayerDamage, float newEnemyDamage) {
    playerDamage = newPlayerDamage;
    enemyDamage = newEnemyDamage;
  }
  Vector3 targetPosition = new Vector3();
  void Move() {
    if(!_isRepuls) {
      targetPosition.x = transform.position.x - speed * Time.deltaTime;
      targetPosition.y = mat.ParabolaGetY(targetPosition.x);
      velocity = (targetPosition - transform.position).normalized;
      transform.position += velocity * speed * Time.deltaTime;
    }else {
      velocity.y -= gravity * Time.deltaTime;
      transform.position += velocity * Time.deltaTime;
    }

    if (transform.position.y <= groundLevel) DestroyObject();
  }

  void Rotation() {

    angle += rotationSpeed * Time.deltaTime;
    graphic.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x , transform.localEulerAngles.y , angle);
    //rb.MoveRotation(angle);
    //graphic.transform.localEulerAngles = Quaternion.Euler(0,0, graphic.transform.localEulerAngles.z + rotationSpeed * Time.deltaTime) * graphic.transform.localEulerAngles;
  }

  void OnTriggerEnter2D(Collider2D target) {
    if (LayerMask.LayerToName(target.gameObject.layer) == "Player") {
      PlayerController player = target.GetComponent<PlayerController>();
      if (player != null) {
        player.Damage(playerDamage);
        DestroyObject();
      }

      //HouseController house = target.GetComponent<HouseController>();
      //if (house != null) {
      //  house.LiveLevelDamade(playerDamage);
      //  DestroyObject();
      //}
    }

    if (_isRepuls && LayerMask.LayerToName(target.gameObject.layer) == "Enemy") {
      if (target.GetComponent<Enemy>())
        target.GetComponent<Enemy>().Damage(gameObject, enemyDamage , 0 , 0);
    }
  }

  /// <summary>
  /// Выполнение отражения
  /// </summary>
  /// <param name="positionFrom">Положение отражаемого объекта</param>
  public void OnRepuls(Vector3 positionFrom) {
    Vector3 repils = (transform.position - positionFrom)*15;
    repils.z = 0;
    repils.y = Mathf.Min(1 , repils.y);
    velocity += repils;
    _isRepuls = true;
  }

  void DestroyObject() {
    gameObject.SetActive(false);
  }
}
