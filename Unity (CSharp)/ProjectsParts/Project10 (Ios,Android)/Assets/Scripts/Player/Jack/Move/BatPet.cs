using System;
using UnityEngine;

namespace Player.Jack {
  
  [Obsolete("Не используется. Актуально PetMove")]
  public class BatPet: PlayerMove {

    public AudioClip activeClip;

    protected override float horizontalSpeed {
      get {
        return 5;
      }
    }

    public override float jumpSpeed {
      get {
        return 5;
      }
    }

    protected override AudioClip boostClip {
      get {
        return activeClip;
      }
    }

    public override void Init() {
      rigidbody.gravityScale = 1;
    }

    public override void DeInit() {
      rigidbody.gravityScale = 6;
    }

    protected override void Animation() {
    }

    protected override void Move() {

      velocity = rigidbody.velocity;
      velocity.x = horizontalKeyValue * horizontalSpeed;
      Debug.Log(velocity);

      // Ограничения горизонтального перемещения
      if ((transform.position.x < CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 0.8f && velocity.x < 0)
          || (transform.position.x > CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 0.8f && velocity.x > 0))
        velocity.x = 0;

      velocity.x += RunnerController.RunSpeed;

      //CalcGravity();

      rigidbody.velocity = velocity;
      if (jumpKey) {
        Jump();
      }

    }

    private void Jump() {
      jumpKey = false;
      //if (controller.isGround) {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
        rigidbody.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
      (pet.instance as Pet.BatPet).WindActive();
      //}
    }

  }
}