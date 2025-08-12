using UnityEngine;

namespace Player.Jack {
  /// <summary>
  /// Колесо мельницы
  /// </summary>
  public class MillWell: PlayerMove {

    private readonly string bigWheelSource = "Prefabs/Busts/Wheel";
    private readonly string fastSpeedsSource = "Prefabs/SFX/RunSpeed";
    private readonly float bigWheelControllerHeight = 2;                   // Высота контроллера
    private GameObject bigWheelInst;                                // Ссылка на созданный объект
    private GameObject bigWellSpeedInst;                            // Буст скорости

    public AudioClip activeClip;

    protected override float horizontalSpeed {
      get { return 5; }
    }

    public override void Init() {
      bigWheelInst = Instantiate(Resources.Load<GameObject>(bigWheelSource), Vector3.zero, Quaternion.identity);
      bigWheelInst.gameObject.SetActive(true);
      bigWellSpeedInst = Instantiate(Resources.Load<GameObject>(fastSpeedsSource), Vector3.zero, Quaternion.identity);
      bigWellSpeedInst.gameObject.SetActive(true);
      bigWellSpeedInst.GetComponent<SpriteRenderer>().sortingOrder = -1;

      controller.boxDamageCollider.offset = new Vector2(-1.2f, 0.77f);
      controller.boxDamageCollider.size = new Vector2(5.7f, 3.24f);
      GetComponent<ShadowController>().SetDiff(Vector3.zero, new Vector3(0.3f, 0.2f, 0));
    }

    protected override void Update() {
      base.Update();

      bigWheelInst.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, 0f);

      bigWellSpeedInst.SetActive(true);
    }

    public override void DeInit() {
      base.DeInit();

      Destroy(bigWheelInst);
      Destroy(bigWellSpeedInst);
    }

    protected override AudioClip boostClip {
      get { return activeClip; }
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

      // Смещаем колеса
        bigWheelInst.transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, 0f);

      // Шлей за большим колесом
        bigWellSpeedInst.transform.position = new Vector3(transform.position.x - 3.4f, transform.position.y - 0.45f, 0f);

      if (controller.isGround) {
        //ResumePlayBoostAudio();

          bigWellSpeedInst.SetActive(true);

        animation.SetAnimation(animation.bigWheeAnim, true);
        controller.graphic.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

      } else {
        controller.graphic.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z);

        animation.SetAnimation(animation.jumpIdleAnim, true);

        if (bigWellSpeedInst)
          bigWellSpeedInst.SetActive(false);

      }

    }

  }

}
