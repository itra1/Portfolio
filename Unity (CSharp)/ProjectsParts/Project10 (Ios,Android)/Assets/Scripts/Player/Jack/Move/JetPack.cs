using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Jack {

  public class JetPack: PlayerMove {
    protected override float horizontalSpeed {
      get {
        throw new System.NotImplementedException();
      }
    }

    protected override AudioClip boostClip {
      get {
        return null;
      }
    }

    public override void Init() {
      throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
  }
}