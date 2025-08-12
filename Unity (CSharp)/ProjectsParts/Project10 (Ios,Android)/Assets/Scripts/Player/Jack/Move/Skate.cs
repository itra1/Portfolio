using UnityEngine;

namespace Player.Jack {

  /// <summary>
  /// Скейт
  /// </summary>
  public class Skate: PlayerMove {

    public GameObject turbo;                                 // Оконь
    private GameObject turboInst;                                   // Клон Объекта
    private readonly string wellBackSource = "Prefabs/Busts/SkateWheelBack";
    private readonly string wellFrontSource = "Prefabs/Busts/SkateWheelFront";
    private GameObject wellBackInst;                           // Ссылка на созданный объект
    private GameObject wellFrontInst;                          // Ссылка на созданный объект
    private readonly float controllerHeight = 1.8f;                      // Высота контроллера
    public AudioClip activeClip;

    protected override float horizontalSpeed { get { return 5; } }

    public override float jumpSpeed { get { return 25; } }

    public override void Init() {
      animation.SetAnimation(animation.skateAnim, true);
      wellBackInst = Instantiate(Resources.Load<GameObject>(wellBackSource), Vector3.zero, Quaternion.identity);
      wellBackInst.gameObject.SetActive(true);
      wellFrontInst = Instantiate(Resources.Load<GameObject>(wellFrontSource), Vector3.zero, Quaternion.identity);
      wellFrontInst.gameObject.SetActive(true);
      turboInst = Instantiate(turbo, Vector3.zero, Quaternion.identity);
      turboInst.gameObject.SetActive(true);
      controller.boxDamageCollider.offset = new Vector2(-0.18f, 0.2f);
      controller.boxDamageCollider.size = new Vector2(1.63f, 2f);
      GetComponent<ShadowController>().SetDiff(Vector3.zero, new Vector3(0.2f, 0.2f, 0));
    }

    protected override void Update() {
      base.Update();

      turboInst.transform.position = new Vector3(transform.position.x - 1f, transform.position.y - 0.3f, 0f);
      wellBackInst.transform.position = new Vector3(transform.position.x - 0.65f, transform.position.y - 0.65f, 0f);
      wellFrontInst.transform.position = new Vector3(transform.position.x + 0.4f, transform.position.y - 0.65f, 0f);

    }

    public override void DeInit() {
      base.DeInit();
      if (turboInst)
        Destroy(turboInst);
      if (wellBackInst)
        Destroy(wellBackInst);
      if (wellFrontInst)
        Destroy(wellFrontInst);
    }

    protected override AudioClip boostClip {
      get { return activeClip; }
    }

    protected override void Move() {

      velocity = rigidbody.velocity;

      velocity.x = RunnerController.Instance.runSpeedActual;

        velocity.x += horizontalKeyValue != 0 ? horizontalSpeed * horizontalKeyValue : 0;

      // Ограничения горизонтального перемещения
      if ((transform.position.x < boundary.min && horizontalKeyValue < 0) || (transform.position.x > boundary.max && horizontalKeyValue > 0))
        velocity.x = RunnerController.Instance.runSpeedActual;

      if (transform.position.x + (velocity.x * Time.deltaTime) < boundary.min)
        velocity.x = RunnerController.Instance.runSpeedActual;

      if (transform.position.x + (velocity.x * Time.deltaTime) > boundary.max)
        velocity.x = RunnerController.Instance.runSpeedActual;

      // Первоначальное появление
      if (controller.playerStart)
        velocity.x = move.RunSpeedToPlayer + 3f;

      if (jumpKey & controller.isGround & !isJumped) {
        isJumped = false;

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

  }

}
