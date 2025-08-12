using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Jack {

  [Obsolete("Не используется. Актуально PetMove")]
  public class SpiderPet: PlayerMove {

    public AudioClip activeClip;

    /// <summary>
    /// Типы положения паука
    /// </summary>
    enum SpiderPosition { down, top };
    /// <summary>
    /// Текущее положение паука
    /// </summary>
    [SerializeField]
    SpiderPosition spiderPosition;

    private Pet.SpiderPet inst;

    protected override float horizontalSpeed {
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
      inst = pet.instance as Pet.SpiderPet;
      spiderPosition = SpiderPosition.top;
      rigidbody.gravityScale = -5;
    }

    protected override void Move() {

      //if (!controller.isGround) {

      //  if (spiderPosition == SpiderPosition.top && inst.graphic.transform.localScale.y == 1) {
      //    //Collider[] isGrounded2 = Physics.OverlapSphere(transform.position, heightDistRevert, groundMaskTop);
      //    if (transform.position.y > CameraController.Instance.transform.position.y) {
      //      inst.graphic.transform.localScale = new Vector3(1, -1, 1);
      //      if (inst.petPhase == Pet.Pet.Phase.jackControl)
      //        PlayerController.Instance.pet.ScalingGraphic(true);
      //    }
      //  } else if (spiderPosition == SpiderPosition.down && inst.graphic.transform.localScale.y == -1) {
      //    //Collider[] isGrounded2 = Physics.OverlapSphere(transform.position, heightDistRevert, groundMask);
      //    if (transform.position.y > CameraController.Instance.transform.position.y) {
      //      inst.graphic.transform.localScale = new Vector3(1, 1, 1);
      //      if (inst.petPhase == Pet.Pet.Phase.jackControl)
      //        PlayerController.Instance.pet.ScalingGraphic(false);
      //      //playerObject.GetComponent<Player.Jack.PlayerPets>().ScalingGraphic(false);
      //    }
      //  }
      //}


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
      if (controller.isGround) {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
        rigidbody.AddForce(new Vector2(0, -jumpSpeed), ForceMode2D.Impulse);
      }
    }

  }
}