using System;
using UnityEngine;
using Spine.Unity;

[System.Serializable]
public enum BoostType {
	none               // Без буста
, speed              // Ускорение
, skate              // Скейт
, barrel             // Бочка
, millWheel          // Большое колесо
, ship               // Корабль
, planer             // Полет
}

namespace Player.Jack {

  public class PlayerBoosters: PlayerComponent {

    bool bustActive;

    #region Links
    Action BoostFixedUpdate;
    Action BoostUpdate;
    bool graf = false;
    BoostType thisBoost;
    RunnerPhase thisRunPhase;
    #endregion

    public AudioClip boosStartClip;
    public AudioClip boosEndClip;

    public float runSpeed = 7;                             // Максимальная скорость горизонтального смещения

    [HideInInspector]
    public float mobileMoveHorizontal;    // Значение мобильного контроллера, отвечающего за горизонтальное смещение
    Vector3 velocity;                        // Рассчет движения
    public float controllerHeight;                          // Стандартная высота контроллера

    [HideInInspector]
    public bool jumpNow;                                    // Флаг, осначающий выполнения врыжка

    public float jumpSpeed = 25;                            // Скорость прыжка
    public float jumpDuration = 0.5f;                       // Время продолжения прыжка
    public float gravity = 65;                              // Графитация, действующая на игрока (только с клавиатуры)
    float forceCrouchEndTime;                               // Время окончания прыжка


    [Header("Буст ускоренного бега")]
    public AudioClip speedClip;
    private readonly float fastHorisontalSpeed = 7;                       // Скорость горизонтального движения
    private readonly string fastCloudSource = "Prefabs/SFX/RunCloud";
    private GameObject fastCloudClone;                                            // Ссылка на созданный объект
    private readonly string fastSpeedsSource = "Prefabs/SFX/RunSpeed";
    private GameObject fastSpeedsClone;                                           // Ссылка на созданный объект
    private readonly string fastSpeedsAnim = "Boost_Barrel";              // Анимация ускоренного бега

    [Header("Буст бочки")]
    public AudioClip barrelClip;
    private readonly float barrierHorisontalSpeed = 5;                    // Скорость горизонтального движения
    private bool barfix = false;                                    // Флаг фиксации бочки
    private readonly string barrierSource = "Prefabs/Busts/Barrel";
    private GameObject barrierClone;                                // Ссылка на созданный объект
    private readonly float barrierControllerHeight = 2.63f;                   // Высота контроллера
    private readonly string boostBarrierAnim = "Boost_Barrel";                    // Анимация бега на бочке

    [Header("Буст колеса")]
    public AudioClip bigWellClip;
    private readonly float bigWheelHorisontalSpeed = 5;                    // Скорость горизонтального движения
    private readonly string bigWheelSource = "Prefabs/Busts/Wheel";
    private GameObject bigWheelClone;                                // Ссылка на созданный объект
    private GameObject bigWellSpeedClone;                            // Буст скорости
    private readonly float bigWheelControllerHeight = 2;                   // Высота контроллера
    private readonly string bigWheeAnim = "Boost_Wheel";                   // Анимация бега на бочке

    [Header("Буст скейта")]
    public GameObject turbo;                                 // Оконь
    private GameObject turboClone;                                   // Клон Объекта
    private readonly float skateHorisontalSpeed = 5;                       // Скорость горизонтального движения
    private readonly string wellBackSkateSource = "Prefabs/Busts/SkateWheelBack";
    private GameObject wellBackSkateClone;                           // Ссылка на созданный объект
    private readonly string wellFrontSkateSource = "Prefabs/Busts/SkateWheelFront";
    private GameObject wellFrontSkateClone;                          // Ссылка на созданный объект
    private readonly float skateControllerHeight = 1.8f;                      // Высота контроллера
    private readonly string skateAnim = "Boost_Skate";                            // Анимация бега на бочке
    public AudioClip skateClip;

    [Header("Буст коробля")]
    public AudioClip shipClip;
    private readonly float shipHorisontalSpeed = 5;                       // Скорость горизонтального движения
    private readonly string wellBackShipSource = "Prefabs/Busts/ShipWheelBack";
    private GameObject wellBackShipClone;                           // Ссылка на созданный объект
    private readonly string wellFrontShipSource = "Prefabs/Busts/ShipWheelFront";
    private GameObject wellFrontShipClone;                          // Ссылка на созданный объект
    private GameObject shipFireClone;                               // Огонь из бочки корабля
    private readonly float shipControllerHeight = 2.6f;                      // Высота контроллера
    private readonly string shipAnim = "Boost_Ship";                            // Анимация бега на бочке

    [Header("Буст полета")]
    public AudioClip flyClip;
    private readonly float flyHorisontalSpeedForvard = 6;                 // Скорость горизонтального движения
    private readonly float flyHorisontalSpeedBack = 4;                    // Скорость горизонтального движения
    private readonly float graphicDiffY = -2.5f;                              // Ссылка на обхект графики
    private readonly string flyAnim = "Boost_Fly";                             // Анимация бега на бочке

    FloatSpan boundary;

    void OnEnable() {
      RunnerController.OnChangeRunnerPhase += ChangePhase;
      thisRunPhase = RunnerPhase.start;
    }

    void OnDisable() {
      RunnerController.OnChangeRunnerPhase -= ChangePhase;
    }

    void ChangePhase(RunnerPhase newPhase) {
      //if (newPhase != RunnerPhase.boost || newPhase != RunnerPhase.postBoost || newPhase != RunnerPhase.start) return;

      if (newPhase == RunnerPhase.boost) {
        PlayBoostAudio();
        AudioManager.PlayEffect(boosStartClip, AudioMixerTypes.runnerEffect);
      }

      thisRunPhase = newPhase;
    }

    public void SetBoost() {
      //player.CreateDust();
      graf = true;

      thisBoost = RunnerController.Instance.booster;

      // В случае не выбранного буста, отменяем
      if (thisBoost == BoostType.none) return;

      if (bustActive) return;

      bustActive = true;

      // Буст ускоренного бега
      if (thisBoost == BoostType.speed) {
        runSpeed = fastHorisontalSpeed;
        fastSpeedsClone = Instantiate(Resources.Load<GameObject>(fastSpeedsSource), Vector3.zero, Quaternion.identity) as GameObject;
        fastCloudClone = Instantiate(Resources.Load<GameObject>(fastCloudSource), Vector3.zero, Quaternion.identity) as GameObject;
        BoostFixedUpdate = RunSpeedMovement;
        BoostUpdate = RunSpeedUpdate;
        boostClip = speedClip;
      }

      // Буст скейта
      if (thisBoost == BoostType.skate) {
        animation.SetAnimation(skateAnim, true);
        wellBackSkateClone = Instantiate(Resources.Load<GameObject>(wellBackSkateSource), Vector3.zero, Quaternion.identity) as GameObject;
        wellFrontSkateClone = Instantiate(Resources.Load<GameObject>(wellFrontSkateSource), Vector3.zero, Quaternion.identity) as GameObject;
        turboClone = Instantiate(turbo, Vector3.zero, Quaternion.identity) as GameObject;
        controller.boxDamageCollider.offset = new Vector2(-0.18f, 0.2f);
        controller.boxDamageCollider.size = new Vector2(1.63f, 2f);
        GetComponent<ShadowController>().SetDiff(Vector3.zero, new Vector3(0.2f, 0.2f, 0));
        runSpeed = skateHorisontalSpeed;
        BoostFixedUpdate = SkateMovement;
        BoostUpdate = SkateMovementUpdate;
        boostClip = skateClip;
      }

      // Буст бочки
      if (thisBoost == BoostType.barrel) {
        barrierClone = Instantiate(Resources.Load<GameObject>(barrierSource), new Vector3(0, -0.1f, 0), Quaternion.identity) as GameObject;
        controller.boxDamageCollider.offset = new Vector2(-2.45f, 0.47f);
        controller.boxDamageCollider.size = new Vector2(5.34f, 2.65f);
        runSpeed = barrierHorisontalSpeed;
        BoostFixedUpdate = BarrierMovement;
        BoostUpdate = BarrelUpdate;
        GetComponent<ShadowController>().Fixed(true);
        controller.graphic.transform.position = new Vector3(transform.position.x, transform.position.y + 0.9f, transform.position.z);
        boostClip = barrelClip;
      }

      // Буст большого колеса
      if (thisBoost == BoostType.millWheel) {

        bigWheelClone = Instantiate(Resources.Load<GameObject>(bigWheelSource), Vector3.zero, Quaternion.identity) as GameObject;
        bigWellSpeedClone = Instantiate(Resources.Load<GameObject>(fastSpeedsSource), Vector3.zero, Quaternion.identity) as GameObject;
        bigWellSpeedClone.GetComponent<SpriteRenderer>().sortingOrder = -1;
        runSpeed = bigWheelHorisontalSpeed;
        controller.boxDamageCollider.offset = new Vector2(-1.2f, 0.77f);
        controller.boxDamageCollider.size = new Vector2(5.7f, 3.24f);
        GetComponent<ShadowController>().SetDiff(Vector3.zero, new Vector3(0.3f, 0.2f, 0));
        BoostFixedUpdate = BarrierMovement;
        BoostUpdate = BarrelUpdate;
        boostClip = bigWellClip;
      }

      // Буст корабля
      if (thisBoost == BoostType.ship) {
        animation.SetAnimation(shipAnim, true);
        wellBackShipClone = Instantiate(Resources.Load<GameObject>(wellBackShipSource), Vector3.zero, Quaternion.identity) as GameObject;
        wellFrontShipClone = Instantiate(Resources.Load<GameObject>(wellFrontShipSource), Vector3.zero, Quaternion.identity) as GameObject;
        shipFireClone = Instantiate(turbo, Vector3.zero, Quaternion.identity) as GameObject;
        GetComponent<ShadowController>().SetDiff(Vector3.zero, new Vector3(0.5f, 0.2f, 0));
        controller.boxDamageCollider.offset = new Vector2(-1.29f, 0.47f);
        controller.boxDamageCollider.size = new Vector2(5.59f, 2.63f);
        controller.boxGroundCollider.offset = new Vector3(-0.54f, -0.87f);
        controller.boxGroundCollider.size = new Vector2(4.32f, 0.005f);
        runSpeed = shipHorisontalSpeed;
        BoostFixedUpdate = BarrierMovement;
        BoostUpdate = BarrelUpdate;
        boostClip = shipClip;
      }

      // Буст Полета
      if (thisBoost == BoostType.planer) {
        animation.SetAnimation(flyAnim, true);
        animation.skeletonAnimation.transform.localPosition = new Vector3(0, graphicDiffY, controller.graphic.transform.localPosition.z);
        //controller.center = new Vector3(0, 0, 0);
        //controller.size = new Vector3(0.6f, 1.76f, 0.6f);
        BoostFixedUpdate = FlyMovement;
        boostClip = flyClip;
      }
    }


    void Update() {
      if (BoostUpdate == null)
        return;
      if (thisRunPhase == RunnerPhase.start) CheckPosition();

      boundary.min = CameraController.displayDiff.leftDif(0.85f);
      boundary.max = CameraController.displayDiff.rightDif(0.85f);

      if (graf) BoostUpdate();

      if ((RunnerController.Instance.runnerPhase & (RunnerPhase.boost | RunnerPhase.preBoost | RunnerPhase.postBoost)) == 0)
        return;

      if (graf) BoostFixedUpdate();

      //ChackCoins();
    }

    //void ChackCoins() {

    //  Ray ray = new Ray(transform.position, Vector3.left);


    //  RaycastHit[] allcoins = Physics.SphereCastAll(ray,1,chackDistance,coinsMask);

    //  foreach(RaycastHit oneHit in allcoins)
    //    oneHit.transform.GetComponent<Coin>().AddCouns();
    //}

    void CheckPosition() {
      if (wellBackSkateClone)
        wellBackSkateClone.transform.position = new Vector3(transform.position.x - 0.65f, transform.position.y - 0.65f, 0f);

      // Переднее колесо
      if (wellFrontSkateClone)
        wellFrontSkateClone.transform.position = new Vector3(transform.position.x + 0.4f, transform.position.y - 0.65f, 0f);

      if (turboClone)
        turboClone.transform.position = new Vector3(transform.position.x - 1f, transform.position.y - 0.3f, 0f);

      if (barrierClone & !barfix)
        barrierClone.transform.position = new Vector3(transform.position.x, transform.position.y - 0.48f, 0f);

      // Смещаем колеса
      if (bigWheelClone)
        bigWheelClone.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, 0f);

      if (bigWellSpeedClone) bigWellSpeedClone.SetActive(true);

      // Смещаем колеса
      if (wellBackShipClone)
        wellBackShipClone.transform.position = new Vector3(transform.position.x - 0.8f, transform.position.y - 0.6f, -0f);

      // Огонь из бочки
      if (shipFireClone)
        shipFireClone.transform.position = new Vector3(transform.position.x - 1.35f, transform.position.y + 0.83f, 0f);

      // Смещаем колеса
      if (wellFrontShipClone)
        wellFrontShipClone.transform.position = new Vector3(transform.position.x + 0.7f, transform.position.y - 0.55f, -0f);

    }

    float vectorX;

    /// <summary>
    /// Буст
    /// </summary>
    void RunSpeedMovement() {
      vectorX = mobileMoveHorizontal;
      velocity = rigidbody.velocity;

      velocity.x = move.RunSpeedToPlayer;

      if (vectorX != 0) {
        velocity.x += runSpeed * Mathf.Sign(vectorX);
      }

      // Ограничения горизонтального перемещения

      if ((transform.position.x < boundary.min && vectorX < 0) || (transform.position.x > boundary.max && vectorX > 0))
        velocity.x = move.RunSpeedToPlayer;

      if (transform.position.x + (velocity.x * Time.deltaTime) < boundary.min)
        velocity.x = move.RunSpeedToPlayer * 1.1f;

      if (transform.position.x + (velocity.x * Time.deltaTime) > boundary.max)
        velocity.x = move.RunSpeedToPlayer * 0.9f;

      // Первоначальное появление
      if (controller.playerStart) {
        velocity.x = move.RunSpeedToPlayer + 3f;
      }

      if (isJump && controller.isGround && !jumpNow) {
        isJump = false;

        rigidbody.velocity = new Vector2(velocity.x, 0);
        rigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);

      } else {
        rigidbody.velocity = velocity;
      }

    }

    void RunSpeedUpdate() {

      if (controller.isGround) {
        ResumePlayBoostAudio();

        animation.SetAnimation(fastSpeedsAnim, true);

        if (fastSpeedsClone && !fastSpeedsClone.activeInHierarchy)
          fastSpeedsClone.SetActive(true);
        if (fastCloudClone && !fastCloudClone.activeInHierarchy)
          fastCloudClone.SetActive(true);

        if (fastSpeedsClone)
          fastSpeedsClone.transform.position = new Vector3(transform.position.x - 2.3f, transform.position.y - 0.5f, 0f);

        if (fastCloudClone)
          fastCloudClone.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, 0f);
      } else {
        PausePlayBoostAudio();

        if (fastSpeedsClone & fastSpeedsClone.activeInHierarchy)
          fastSpeedsClone.SetActive(false);
        if (fastCloudClone & fastCloudClone.activeInHierarchy)
          fastCloudClone.SetActive(false);
        animation.SetAnimation(animation.jumpIdleAnim, true);
      }

    }


    void BarrierMovement() {

      vectorX = mobileMoveHorizontal;
      velocity = rigidbody.velocity;

      velocity.x = RunnerController.Instance.runSpeedActual;

      if (vectorX != 0) velocity.x += runSpeed * Mathf.Sign(vectorX);

      if (controller.playerStart)
        velocity.x = RunnerController.Instance.runSpeedActual + 3f;

      // Ограничения горизонтального перемещения
      if ((transform.position.x < boundary.min && vectorX < 0) || (transform.position.x > boundary.max && vectorX > 0))
        velocity.x = RunnerController.Instance.runSpeedActual;

      if (transform.position.x + (velocity.x * Time.deltaTime) < boundary.min)
        velocity.x = RunnerController.Instance.runSpeedActual;

      if (transform.position.x + (velocity.x * Time.deltaTime) > boundary.max)
        velocity.x = RunnerController.Instance.runSpeedActual;

      if (isJump & controller.isGround & !jumpNow) {
        isJump = false;

        rigidbody.velocity = new Vector2(velocity.x, 0);
        rigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
      } else {
        rigidbody.velocity = velocity;
      }

    }

    void BarrelUpdate() {

      if (controller.isGround)
        ResumePlayBoostAudio();
      else
        PausePlayBoostAudio();

      // Смещаем бочки
      if (barrierClone & !barfix)
        barrierClone.transform.position = new Vector3(transform.position.x, transform.position.y - 0.38f, 0f);
      else if (barrierClone)
        barrierClone.transform.position = new Vector3(transform.position.x, barrierClone.transform.position.y, 0f);

      // Смещаем колеса
      if (bigWheelClone)
        bigWheelClone.transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, 0f);

      // Шлей за большим колесом
      if (bigWellSpeedClone)
        bigWellSpeedClone.transform.position = new Vector3(transform.position.x - 3.4f, transform.position.y - 0.45f, 0f);

      // Смещаем колеса
      if (wellBackShipClone)
        wellBackShipClone.transform.position = new Vector3(transform.position.x - 0.8f, transform.position.y - 0.45f, -0f);

      // Огонь из бочки
      if (shipFireClone)
        shipFireClone.transform.position = new Vector3(transform.position.x - 1.35f, transform.position.y + 0.83f, 0f);

      // Смещаем колеса
      if (wellFrontShipClone)
        wellFrontShipClone.transform.position = new Vector3(transform.position.x + 0.7f, transform.position.y - 0.45f, -0f);

      if (controller.isGround) {
        //ResumePlayBoostAudio();

        if (bigWellSpeedClone)
          bigWellSpeedClone.SetActive(true);

        if (thisBoost == BoostType.barrel) {
          barfix = false;
          animation.SetAnimation(boostBarrierAnim, true);
        }

        if (thisBoost == BoostType.millWheel) {
          animation.SetAnimation(bigWheeAnim, true);
          controller.graphic.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
      } else {
        if (thisBoost == BoostType.millWheel)
          controller.graphic.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z);

        if (thisBoost == BoostType.millWheel | thisBoost == BoostType.barrel)
          animation.SetAnimation(animation.jumpIdleAnim, true);
        if (thisBoost == BoostType.barrel)
          barfix = true;

        if (bigWellSpeedClone)
          bigWellSpeedClone.SetActive(false);

      }

    }

    void SkateMovement() {
      vectorX = mobileMoveHorizontal;
      velocity = rigidbody.velocity;

      velocity.x = RunnerController.Instance.runSpeedActual;

      if (vectorX != 0) {
        velocity.x += runSpeed * Mathf.Sign(vectorX);
      }

      // Ограничения горизонтального перемещения
      if ((transform.position.x < boundary.min && vectorX < 0) || (transform.position.x > boundary.max && vectorX > 0))
        velocity.x = RunnerController.Instance.runSpeedActual;

      if (transform.position.x + (velocity.x * Time.deltaTime) < boundary.min)
        velocity.x = RunnerController.Instance.runSpeedActual;

      if (transform.position.x + (velocity.x * Time.deltaTime) > boundary.max)
        velocity.x = RunnerController.Instance.runSpeedActual;

      // Первоначальное появление
      if (controller.playerStart)
        velocity.x = move.RunSpeedToPlayer + 3f;

      if (isJump && controller.isGround && !jumpNow) {
        isJump = false;

        rigidbody.velocity = new Vector2(velocity.x, 0);
        rigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);

      } else {
        rigidbody.velocity = velocity;
      }




      // Двигаем
      //controller.Move(new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime);
      /*
      CheckPosition(transform.position + velocity * Time.deltaTime + new Vector3(0 , -0.858f , 0));

      if (isGroungs && velocity.y < 0) {
          velocity.y = 0;
          transform.position = new Vector3(transform.position.x + velocity.x * Time.deltaTime , isGroungsArray[0].transform.position.y + 0.858f , transform.position.z);
      } else
          transform.position += velocity * Time.deltaTime;
      */
      //rb.velocity = velocity;


    }

    void SkateMovementUpdate() {

      if (controller.isGround)
        ResumePlayBoostAudio();
      else
        PausePlayBoostAudio();

      //if (turboClone)
      //	turboClone.transform.position = new Vector3(transform.position.x - 1f, transform.position.y - 0.3f, 0f);

      // Заднее колесо
      if (wellBackSkateClone)
        wellBackSkateClone.transform.position = new Vector3(transform.position.x - 0.65f, transform.position.y - 0.67f, 0f);

      // Переднее колесо
      if (wellFrontSkateClone)
        wellFrontSkateClone.transform.position = new Vector3(transform.position.x + 0.4f, transform.position.y - 0.67f, 0f);

    }




    void FlyMovement() {
      float x = 0;

      x = mobileMoveHorizontal;

      velocity.x = 0;

      if (x != 0) {
        velocity.x = (x < 0) ? runSpeed : flyHorisontalSpeedForvard;
        velocity.x = (x > 0) ? runSpeed : flyHorisontalSpeedBack;
        velocity.x *= Mathf.Sign(x);
      }

      if (controller.isGround && !isJump)
        velocity.y = -gravity * Time.deltaTime;

      // Гравитационное действие
      if (velocity.x < 0 || isJump)
        velocity.y += (gravity * Time.deltaTime) / 3f;

      if (velocity.x >= 0 && !isJump)
        velocity.y -= (gravity * Time.deltaTime) / 4;

      if (velocity.y > 0) {
        if (controller.graphic.transform.eulerAngles.z < 30)
          controller.graphic.transform.eulerAngles = new Vector3(0, 0, controller.graphic.transform.eulerAngles.z + 1f);

        if (controller.graphic.transform.eulerAngles.z == 30)
          controller.graphic.transform.eulerAngles = new Vector3(0, 0, 30);
      } else {
        if (controller.graphic.transform.eulerAngles.z > 0)
          controller.graphic.transform.eulerAngles = new Vector3(0, 0, controller.graphic.transform.eulerAngles.z - 1f);

        if (controller.graphic.transform.eulerAngles.z > 30)
          controller.graphic.transform.eulerAngles = new Vector3(0, 0, 0);
      }

      // Добавим ограничение скорости спуска
      velocity.y = velocity.y <= -4 ? -4 : velocity.y;
      // Ограничение скорости подема
      velocity.y = velocity.y >= 4 ? 4 : velocity.y;

      // Ограничение по максимальной высоте
      velocity.y = (transform.position.y >= 10.5f && velocity.y > 0) ? 0 : velocity.y;

      // Запрет опускаться слишком низко
      bool isGround = Physics.CheckSphere(transform.position, 1f, controller.groundLayer);
      if (isGround && velocity.y < 0) velocity.y = 0;

      // Первоначальное появление
      if (controller.playerStart) velocity.x = 3f;
      if (controller.playerStart) velocity.y = 0f;

      // Двигаем
      //controller.Move(new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime);

      //CheckPosition(transform.position + velocity * Time.deltaTime + new Vector3(0 , -0.858f , 0));
      rigidbody.velocity = velocity;

      if (controller.isGround && velocity.y < 0) {
        velocity.y = 0;
        //transform.position = new Vector3(transform.position.x + velocity.x * Time.deltaTime, player.isGroungsArray[0].transform.position.y + 0.858f, transform.position.z);
      } else
        transform.position += velocity * Time.deltaTime;

      // Ограничения горизонтального перемещения
      if (transform.position.x < boundary.min && velocity.x < 0)
        transform.position = new Vector3(boundary.min, transform.position.y, transform.position.z);

      // Ограничения горизонтального перемещения
      if (transform.position.x > boundary.max && velocity.x > 0)
        transform.position = new Vector3(boundary.max, transform.position.y, transform.position.z);

    }

    Collider[] isGroungsArray;
    /*
    void CheckPosition(Vector3 checkPosition) {
        isGroungsArray = Physics.OverlapSphere(checkPosition , player.groundRadius , player.groundLayer);
        isGroungs = isGroungsArray.Length > 0;
    }
    */

    /// <summary>
    /// Оключаем бусты
    /// </summary>
    public void EndBoosters() {
      audioCompBoost.Stop();

      if (fastCloudClone)
        Destroy(fastCloudClone);
      if (fastSpeedsClone)
        Destroy(fastSpeedsClone);
      if (barrierClone)
        Destroy(barrierClone);
      if (bigWheelClone)
        Destroy(bigWheelClone);
      if (wellBackSkateClone)
        Destroy(wellBackSkateClone);
      if (wellFrontSkateClone)
        Destroy(wellFrontSkateClone);
      if (wellBackShipClone)
        Destroy(wellBackShipClone);
      if (wellFrontShipClone)
        Destroy(wellFrontShipClone);
      if (turboClone)
        Destroy(turboClone);
      if (bigWellSpeedClone)
        Destroy(bigWellSpeedClone);
      if (shipFireClone)
        Destroy(shipFireClone);


      GetComponent<ShadowController>().Fixed(false);
      GetComponent<ShadowController>().SetDeff();

      controller.boxDamageCollider.offset = Vector2.zero;
      controller.boxDamageCollider.size = new Vector2(0.44f, 1.7f);

      controller.boxGroundCollider.offset = new Vector3(0, 0f);
      controller.boxGroundCollider.size = new Vector2(0.44f, 0.005f);

      controller.graphic.transform.localPosition = new Vector3(0, 0, controller.graphic.transform.localPosition.z);
      controller.graphic.transform.localEulerAngles = new Vector3(0, 0, 0);
      animation.skeletonAnimation.transform.localPosition = new Vector3(0, -0.88f, animation.skeletonAnimation.transform.localPosition.z);
      animation.ResetAnimation();
      BoostUpdate = null;
      BoostFixedUpdate = null;

    }

    /// <summary>
    /// Движение
    /// </summary>
    /// <param name="mov"></param>
    public void Movement(float mov) {
      mobileMoveHorizontal = mov;
    }

    bool isJump;

    /// <summary>
    /// Подпрыгивание
    /// </summary>
    /// <param name="flag"></param>
    public void Jump(bool flag) {
      isJump = flag;
    }

    #region Audio

    AudioClip boostClip;
    public AudioSource audioCompBoost;                  // Компонент аудио


    void PlayBoostAudio() {
      if (boostClip != null & audioCompBoost.clip != boostClip) {
        audioCompBoost.clip = boostClip;
        audioCompBoost.Play();
      }
    }

    void ResumePlayBoostAudio() {
      if (!audioCompBoost.isPlaying)
        audioCompBoost.UnPause();
    }

    void PausePlayBoostAudio() {
      if (audioCompBoost.isPlaying)
        audioCompBoost.Pause();
    }



    #endregion
  }
}