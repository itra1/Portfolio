using UnityEngine;

namespace Player.Jack {
  /// <summary>
  /// Ускоренный бег
  /// </summary>
  public class Force: PlayerMove {

    private readonly string cloudSource = "Prefabs/SFX/RunCloud";
    private readonly string speedsSource = "Prefabs/SFX/RunSpeed";
    private GameObject speedsInst;                             // Ссылка на созданный объект
    private GameObject cloudInst;

    public AudioClip activeAudio;

    public override void Init() {
      speedsInst = Instantiate(Resources.Load<GameObject>(speedsSource), Vector3.zero, Quaternion.identity);
      speedsInst.SetActive(true);
      cloudInst = Instantiate(Resources.Load<GameObject>(cloudSource), Vector3.zero, Quaternion.identity);
      cloudInst.SetActive(true);
    }

    protected override void Update() {
      base.Update();

      //speedsInst.transform.position = new Vector3(transform.position.x - 2.3f, transform.position.y - 0.5f, 0f);

      //cloudInst.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, 0f);
    }

    protected override float horizontalSpeed {
      get { return 7; }
    }

    protected override AudioClip boostClip {
      get { return activeAudio; }
    }

    public override void DeInit() {
      audioCompBoost.Stop();

      if (speedsInst)
        Destroy(speedsInst);
      if (cloudInst)
        Destroy(cloudInst);
    }

    protected override void Move() {
      
      velocity = rigidbody.velocity;

      velocity.x = move.RunSpeedToPlayer;

      velocity.x += horizontalKeyValue != 0 ? horizontalKeyValue * horizontalSpeed : 0;

      // Ограничения горизонтального перемещения

      if ((transform.position.x < boundary.min && horizontalKeyValue < 0) || (transform.position.x > boundary.max && horizontalKeyValue > 0))
        velocity.x = move.RunSpeedToPlayer;

      if (transform.position.x + (velocity.x * Time.deltaTime) < boundary.min)
        velocity.x = move.RunSpeedToPlayer * 1.1f;

      if (transform.position.x + (velocity.x * Time.deltaTime) > boundary.max)
        velocity.x = move.RunSpeedToPlayer * 0.9f;

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

    }

    
    protected override void Animation() {

      if (controller.isGround) {
        ResumePlayBoostAudio();

        animation.SetAnimation(animation.fastSpeedsAnim, true);

        if (!speedsInst.activeInHierarchy)
          speedsInst.SetActive(true);
        if (!cloudInst.activeInHierarchy)
          cloudInst.SetActive(true);

        speedsInst.transform.position = new Vector3(transform.position.x - 2.3f, transform.position.y - 0.5f, 0f);
        cloudInst.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, 0f);

      } else {
        PausePlayBoostAudio();

        if (speedsInst.activeInHierarchy)
          speedsInst.SetActive(false);
        if (cloudInst.activeInHierarchy)
          cloudInst.SetActive(false);
        animation.SetAnimation(animation.jumpIdleAnim, true);
      }

    }

  }

}