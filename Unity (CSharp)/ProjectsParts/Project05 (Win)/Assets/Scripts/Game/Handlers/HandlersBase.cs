using UnityEngine;
using System.Collections;
namespace it.Game.Handles
{
  public class HandlersBase : MonoBehaviourBase
  {
	 [SerializeField]
	 private bool _isActived = true;

	 public bool IsActived { get => _isActived; set => _isActived = value; }

	 private void Update()
	 {
		if (!IsActived)
		  return;
		OnUpdate();
	 }

	 protected virtual void OnUpdate()
	 {

	 }

  }
}