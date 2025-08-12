using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Jack {

  public class PetMove: PlayerMove {
    protected override float horizontalSpeed {
      get {
        return 0;
      }
    }

    protected override AudioClip boostClip {
      get {
        return null;
      }
    }

    public override void Init() {
      rigidbody.gravityScale = 0;
    }

    public override void DeInit() {
      base.DeInit();
      rigidbody.gravityScale = 6;
    }

    public override bool jumpKey {
      get {
        return pet.instance.jumpKey;
      }

      set {
        pet.instance.jumpKey = value;
      }
    }

    public override float horizontalKey {
      set {
        pet.instance.horizontalKey = value;
      }
    }

    protected override void Move() {
    }
  }
}