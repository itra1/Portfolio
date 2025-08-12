using UnityEngine;

namespace Player.Jack {
  /// <summary>
  /// Корабль
  /// </summary>
  public class Ship: PlayerMove {

    public GameObject turbo;                                 // Оконь
    private readonly string wellBackSource = "Prefabs/Busts/ShipWheelBack";
    private readonly string wellFrontSource = "Prefabs/Busts/ShipWheelFront";
    private GameObject wellBackInst;                           // Ссылка на созданный объект
    private GameObject wellFrontInst;                          // Ссылка на созданный объект
    private GameObject shipFireInst;                               // Огонь из бочки корабля
    private readonly float shipControllerHeight = 2.6f;                      // Высота контроллера

    public AudioClip activeClip;

    protected override float horizontalSpeed { get { return 5; } }

    public override void Init() {
      animation.SetAnimation(animation.shipAnim, true);
      wellBackInst = Instantiate(Resources.Load<GameObject>(wellBackSource), Vector3.zero, Quaternion.identity);
      wellBackInst.gameObject.SetActive(true);
      wellFrontInst = Instantiate(Resources.Load<GameObject>(wellFrontSource), Vector3.zero, Quaternion.identity);
      wellFrontInst.gameObject.SetActive(true);
      shipFireInst = Instantiate(turbo, Vector3.zero, Quaternion.identity);
      shipFireInst.gameObject.SetActive(true);
      GetComponent<ShadowController>().SetDiff(Vector3.zero, new Vector3(0.5f, 0.2f, 0));
      controller.boxDamageCollider.offset = new Vector2(-1.29f, 0.47f);
      controller.boxDamageCollider.size = new Vector2(5.59f, 2.63f);
      controller.boxGroundCollider.offset = new Vector3(-0.54f, -0.87f);
      controller.boxGroundCollider.size = new Vector2(4.32f, 0.005f);
    }

    protected override void Update() {
      base.Update();

      shipFireInst.transform.position = new Vector3(transform.position.x - 1.35f, transform.position.y + 0.83f, 0f);
      wellFrontInst.transform.position = new Vector3(transform.position.x + 0.7f, transform.position.y - 0.55f, -0f);
      wellBackInst.transform.position = new Vector3(transform.position.x - 0.8f, transform.position.y - 0.6f, -0f);
    }

    public override void DeInit() {
      base.DeInit();

      if (wellBackInst)
        Destroy(wellBackInst);
      if (wellFrontInst)
        Destroy(wellFrontInst);
      if (shipFireInst)
        Destroy(shipFireInst);
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

      shipFireInst.transform.position = new Vector3(transform.position.x - 1.35f, transform.position.y + 0.83f, 0f);
      wellFrontInst.transform.position = new Vector3(transform.position.x + 0.7f, transform.position.y - 0.55f, -0f);
      wellBackInst.transform.position = new Vector3(transform.position.x - 0.8f, transform.position.y - 0.6f, -0f);

    }

  }

}