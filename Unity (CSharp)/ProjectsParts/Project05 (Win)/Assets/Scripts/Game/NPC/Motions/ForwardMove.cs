using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.NPC.Motions {
  public class ForwardMove : MonoBehaviour {
    [Tooltip("Скорлсть перемещения")]
    [SerializeField]
    private float _speed;

    public float Speed { get => _speed; set => _speed = value; }

    private void Update() {

      transform.position += transform.forward * Speed * Time.deltaTime;

    }

  }
}