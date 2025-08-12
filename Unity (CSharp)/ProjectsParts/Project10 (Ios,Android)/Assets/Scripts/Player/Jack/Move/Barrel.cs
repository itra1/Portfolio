using UnityEngine;

namespace Player.Jack {
  /// <summary>
  /// Бочка
  /// </summary>
  public sealed class Barrel: PlayerMove {

    private bool barfix = false;                                    // Флаг фиксации бочки
                                                                    //public GameObject barrier;                              // Объект бочки
    private readonly string barrierSource = "Prefabs/Busts/Barrel";
    private readonly float barrierControllerHeight = 2.63f;                   // Высота контроллера
    private GameObject barrierInst;                                // Ссылка на созданный объект
    public AudioClip activeClip;

    protected override float horizontalSpeed {
      get { return 5; }
    }

    protected override void Update() {

      barfix = !controller.isGround;

      if (!barfix)
        animation.SetAnimation(animation.boostBarrierAnim, true);

      base.Update();

      barrierInst.transform.position = new Vector3(transform.position.x, transform.position.y - 0.38f, 0f);
      barrierInst.transform.position = new Vector3(transform.position.x, barrierInst.transform.position.y, 0f);
    }

    public override void Init() {
      barrierInst = Instantiate(Resources.Load<GameObject>(barrierSource), new Vector3(0, -0.1f, 0), Quaternion.identity);
      controller.boxDamageCollider.offset = new Vector2(-2.45f, 0.47f);
      controller.boxDamageCollider.size = new Vector2(5.34f, 2.65f);
      GetComponent<ShadowController>().Fixed(true);
      controller.graphic.transform.position = new Vector3(transform.position.x, transform.position.y + 0.9f, transform.position.z);
    }

    protected override AudioClip boostClip {
      get { return activeClip; }
    }

    public override void DeInit() {
      if (barrierInst)
        Destroy(barrierInst);
    }

    protected override void Move() {
      velocity = rigidbody.velocity;

      velocity.x = RunnerController.Instance.runSpeedActual;

      velocity.x += horizontalKeyValue != 0 ? horizontalSpeed * horizontalKeyValue : 0;

      if (controller.playerStart)
        velocity.x = RunnerController.Instance.runSpeedActual + 3f;

      // Ограничения горизонтального перемещения
      if ((transform.position.x < boundary.min && horizontalKeyValue < 0) || (transform.position.x > boundary.max && horizontalKeyValue > 0))
        velocity.x = RunnerController.Instance.runSpeedActual;

      if (transform.position.x + (velocity.x * Time.deltaTime) < boundary.min)
        velocity.x = RunnerController.Instance.runSpeedActual;

      if (transform.position.x + (velocity.x * Time.deltaTime) > boundary.max)
        velocity.x = RunnerController.Instance.runSpeedActual;

      if (jumpKey & controller.isGround & !isJumped) {
        isJumped = false;

        rigidbody.velocity = new Vector2(velocity.x, 0);
        rigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
      } else {
        rigidbody.velocity = velocity;
      }
    }

    protected override void Animation() {

      if (controller.isGround)
        ResumePlayBoostAudio();
      else
        PausePlayBoostAudio();

      // Смещаем бочки
      if (barrierInst & !barfix)
        barrierInst.transform.position = new Vector3(transform.position.x, transform.position.y - 0.38f, 0f);
      else if (barrierInst)
        barrierInst.transform.position = new Vector3(transform.position.x, barrierInst.transform.position.y, 0f);

      if (controller.isGround) {

        barfix = false;
        animation.SetAnimation(animation.boostBarrierAnim, true);

      } else {

        animation.SetAnimation(animation.jumpIdleAnim, true);
        barfix = true;

      }

    }

  }

}