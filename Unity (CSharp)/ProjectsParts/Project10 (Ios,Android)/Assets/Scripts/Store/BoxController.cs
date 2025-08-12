using UnityEngine;

/// <summary>
/// Контроллер бонусного ящика
/// </summary>
public class BoxController : MonoBehaviour {
  delegate void MoveFunc();
  MoveFunc MoveFunction;

  void OnEnable() {
    InitMoveType();
    InitEffect();
  }

  void OnDisable() {
    MoveFunction = null;

    if(trailInstance != null)
      Destroy(trailInstance.gameObject);

  }

  void Update() {
    Move();
  }
  /// <summary>
  /// Обработка вхождения в триггер
  /// </summary>
  /// <param name="other"></param>
  void OnTriggerEnter2D(Collider2D other) {
    if(other.tag == "Player" || other.tag == "Pet") {

      if(sfxClip)
        AudioManager.PlayEffect(sfxClip, AudioMixerTypes.runnerEffect);

      BoxSpawner.instance.GenerateTakeEffect(transform.position + new Vector3(0, 0.5f, 0));

      oper(other);

      if(trailInstance != null)
        Destroy(trailInstance);

      gameObject.SetActive(false);
    }
  }

  #region Движение


  /// <summary>
  /// Скорость горизонтального смещения при полете
  /// </summary>
  public FloatSpan diffSpeedX;
  /// <summary>
  /// Диапазон скорости смещения по вертикали при полете
  /// </summary>
  public FloatSpan diffSpeedY;
  /// <summary>
  /// Стартовое значение вертикального смещения
  /// </summary>
  public FloatSpan startSpeedY;
  /// <summary>
  /// Диапазон максимальных вертикальных отклонений при котором изменяется вектор ускорения
  /// </summary>
  public FloatSpan diffY;


  [SerializeField]
  float speedYDiff;

  /// <summary>
  /// Максимальная вертикальная скорость при полете
  /// </summary>
  float maxSpeedY;

  /// <summary>
  /// Вестор жвижения
  /// </summary>
  [SerializeField]
  Vector3 velocity;

  /// <summary>
  /// Слой с поверхностью земли
  /// </summary>
  [SerializeField]
  LayerMask groundMask;

  /// <summary>
  /// Стартовая позиция по вертикальной оси
  /// </summary>
  float startYPosition;

  /// <summary>
  /// Максимальное вертикальное откланение при котором изменяется вектор ускорения
  /// </summary>
  float maxDiffY;

  /// <summary>
  /// Делегат и событие по уничтожения ящика при достижении левой границы
  /// </summary>
  //public event EventDelegate OnLeftDestroy;
  //public delegate void EventDelegate();

  /// <summary>
  /// Направление смещения по вертикали
  /// </summary>
  float vector = 1;

  /// <summary>
  /// Инициализация типа движения ящика (лежит/летит)
  /// </summary>
  void InitMoveType() {

    if(MoveFunction != null)
      return;

    startYPosition = transform.position.y;
    if(Random.value <= 0.5f) {
      MoveFunction = null;
      DownNow();
    } else {
      MoveFunction = MoveSinusFly;
      FlyRondom();
    }
  }

  /// <summary>
  /// Обработка движения каждый кадр
  /// </summary>
  void Move() {

    if(MoveFunction != null)
      MoveFunction();

    if(transform.position.x <= CameraController.displayDiff.leftDif(1.5f)) {

      //if(OnLeftDestroy != null) OnLeftDestroy();

      if(trailInstance != null)
        Destroy(trailInstance);

			ExEvent.GameEvents.BoxLeftDestroy.CallAsync(this);

      //OnLeftDestroy = null;
      gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Зафиксировать на полу
  /// </summary>
  void DownNow() {

		try {
			transform.Find("Trail").GetComponent<TrailRenderer>().Clear();
		} catch { }

		RaycastHit2D point = Physics2D.Raycast(transform.position, Vector3.down,5, groundMask);

		if (point) {
			transform.position = point.collider.transform.position;
			return;
		}

		//RaycastHit2D[] groundObject = Physics2D.RaycastAll(transform.position, Vector3.down*10, groundMask);

		//if(groundObject.Length > 0) {
		//  for(int i = 0; i < groundObject.Length; i++) {
		//    if(LayerMask.LayerToName(groundObject[i].collider.gameObject.layer) == "ground") continue;
		//    transform.position = groundObject[i].transform.position;
		//    //if (transform.Find("Trail")) transform.Find("Trail").GetComponent<TrailRenderer>().enabled = false;
		//    return;
		//  }
		//}

		// Если пришли сюда, значит не нашли на что поместить препятствие
		MoveFunction = MoveSinusFly;
    FlyRondom();
  }

  /// <summary>
  /// Рассчет полета
  /// </summary>
  void MoveSinusFly() {
    if(vector > 0 && transform.position.y > startYPosition + maxDiffY) {
      vector = -1;
    } else if(vector < 0 && transform.position.y < startYPosition - maxDiffY) {
      vector = 1;
    }

    velocity.y += speedYDiff * vector * Time.deltaTime;
    if(Mathf.Abs(velocity.y) > maxSpeedY) velocity.y = maxSpeedY * Mathf.Sign(velocity.y);
    transform.position += velocity * Time.deltaTime;
  }

  GameObject trailInstance;

  /// <summary>
  /// Рассчет траектории полета
  /// </summary>
  void FlyRondom() {
    Transform tr = transform.Find("Trail");
    if(tr != null) {

      trailInstance = Instantiate(tr.gameObject, tr.position, Quaternion.identity) as GameObject;
      trailInstance.transform.parent = tr.parent;
      trailInstance.SetActive(true);
    }

    velocity.x = RunnerController.RunSpeed + Random.Range(diffSpeedX.min, diffSpeedX.max);
    maxSpeedY = Random.Range(startSpeedY.min, startSpeedY.max);
    maxDiffY = Random.Range(diffY.min, diffY.max);
    vector = Random.value <= 0.5f ? 1 : -1;
    velocity.y = maxSpeedY;
    //_timeCoeff = Random.Range(timeCoeffSpan.min, timeCoeffSpan.max);
    speedYDiff = Random.Range(diffSpeedY.min, diffSpeedY.max);
  }
  #endregion

  #region Эффект

  /// <summary>
  /// Делегат выполняемой операции
  /// </summary>
  /// <param name="other"></param>
  delegate void CollisionOperate(Collider2D other);
  CollisionOperate oper;

  /// <summary>
  /// Тип ящика
  /// </summary>
  public BoxType type;

  /// <summary>
  /// Звуковой эффект взятия ящика
  /// </summary>
  public AudioClip sfxClip;

  void InitEffect() {
    if(type == BoxType.weaponTrap
        || type == BoxType.weaponSabel
        || type == BoxType.weaponGun
        || type == BoxType.weaponBomb
        || type == BoxType.weaponMolotov
        || type == BoxType.weaponShip
        || type == BoxType.weaponChocolate
        || type == BoxType.weaponFlowers
        || type == BoxType.weaponCandy

       )
      oper = new CollisionOperate(AddWeaponToPlayer);

    if(type == BoxType.bullet)
      oper = new CollisionOperate(MagicBullet);
    if(type == BoxType.pirat)
      oper = new CollisionOperate(MagicPirats);

    if(type == BoxType.blackPoint)
      oper = new CollisionOperate(AddBlackPoint);
    if(type == BoxType.cristall)
      oper = new CollisionOperate(AddCristall);
    if(type == BoxType.magnet)
      oper = new CollisionOperate(ActivateMagnet);
    if(type == BoxType.hearth)
      oper = new CollisionOperate(AddHearth);
    if(type == BoxType.power)
      oper = new CollisionOperate(AddPower);
    if(type == BoxType.shield)
      oper = new CollisionOperate(AddShield);
  }


  /// <summary>
  /// Добавляем оружие игроку
  /// </summary>
  /// <param name="other">Коллайлер игрока</param>
  void AddWeaponToPlayer(Collider2D other) {
    RunnerController.Instance.WeaponAdd(type);
  }

  /// <summary>
  /// Добавляем кристаллы игроку
  /// </summary>
  /// <param name="other">Коллайлер игрока</param>
  void AddCristall(Collider2D other) {
    RunnerController.Instance.cristallInRace++;
  }

  /// <summary>
  /// Добавляем черную метку игроку
  /// </summary>
  /// <param name="other">Коллайлер игрока</param>
  void AddBlackPoint(Collider2D other) {
    RunnerController.Instance.AddBlackMark(1);
  }

  /// <summary>
  /// Добавляем магнит игроку
  /// </summary>
  /// <param name="other">Коллайлер игрока</param>
  void ActivateMagnet(Collider2D other) {
    other.GetComponent<Player.Jack.PlayerController>().Magnet(true);
  }

  /// <summary>
  /// Добавляем жизни игроку
  /// </summary>
  /// <param name="other">Коллайлер игрока</param>
  void AddHearth(Collider2D other) {
    //Questions.QuestionManager.addHeart(other.transform.position);
    Questions.QuestionManager.ConfirmQuestion(Quest.getHearth, 1, other.transform.position);
    RunnerController.Instance.PlayerAddLive(1);
  }

  /// <summary>
  /// Добавляем энергию игроку
  /// </summary>
  /// <param name="other">Коллайлер игрока</param>
  void AddPower(Collider2D other) {
    RunnerController.Instance.AddPower(20);
  }

  /// <summary>
  /// Активация магии пиратов
  /// </summary>
  /// <param name="other">Коллайлер игрока</param>
  void MagicPirats(Collider2D other) {
    MagicSpawner.Instance.Magic(MagicTypes.pirats);

  }

  /// <summary>
  /// Активация магии пушечных ядер
  /// </summary>
  /// <param name="other">Коллайлер игрока</param>
  void MagicBullet(Collider2D other) {
    MagicSpawner.Instance.Magic(MagicTypes.bullet);
  }


  void AddShield(Collider2D other) {
    other.GetComponent<Player.Jack.PlayerController>().Shield(true);
  }

  #endregion
  /// <summary>
  /// Установка ящика как падающего сверху
  /// </summary>
  public void SetDropDown() {
    velocity.x = RunnerController.RunSpeed;
    velocity.y = 0;
    MoveFunction = MoveDropDown;
  }

  Collider[] groundObject;

  void MoveDropDown() {
    velocity.x = RunnerController.RunSpeed;
    velocity.y -= 15 * Time.deltaTime;

    transform.position += velocity * Time.deltaTime;

    groundObject = Physics.OverlapSphere(transform.position, 1, groundMask);

    if(groundObject.Length > 0) {
      for(int i = 0; i < groundObject.Length; i++) {
        //if (LayerMask.LayerToName(groundObject[i].gameObject.layer) == "ground")
        //    continue;
        transform.position = new Vector3(transform.position.x, groundObject[i].transform.position.y, transform.position.z);

        MoveFunction = null;
      }
    }

  }

}
