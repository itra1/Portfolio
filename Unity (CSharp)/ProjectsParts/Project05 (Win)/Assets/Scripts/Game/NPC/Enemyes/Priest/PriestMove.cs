using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriestMove : MonoBehaviour
{
  private com.ootii.Actors.ActorController _actorController;
  [SerializeField]
  private float _speed = 5;
  [SerializeField]
  private Transform _camera;
  [SerializeField]
  private float _speedCamera = 5;

  private bool _move = false;
  private void Start()
  {
    _actorController = GetComponent<com.ootii.Actors.ActorController>();
    _camera.gameObject.SetActive(false);


  }

  public void Active()
  {
    CameraBehaviour.Instance.gameObject.SetActive(false);
    _camera.gameObject.SetActive(true);
    _move = true;
  }

  private void Update()
  {
    if (!_move) return;
    _actorController.Move(transform.forward * Time.deltaTime * _speed);
    _camera.transform.localPosition += transform.up * _speedCamera * Time.deltaTime;
  }

}
