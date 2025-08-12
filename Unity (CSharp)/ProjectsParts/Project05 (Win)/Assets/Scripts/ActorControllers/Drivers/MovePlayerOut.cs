using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPlayer = it.Game.Player;

namespace com.ootii.Actors
{
  public class MovePlayerOut : MonoBehaviourBase
  {
	 protected MoveToDriver mDriver = null;
	 public void Awake()
	 {
		mDriver = gameObject.GetComponent<MoveToDriver>();
	 }
	 private void Start()
	 {
		
	 }

	 private void Update()
	 {
		if (GPlayer.PlayerBehaviour.Instance == null)
		  return;
		Vector3 forward = GPlayer.PlayerBehaviour.Instance.transform.forward;
		Vector3 target = transform.position + forward * 100;
	 }

  }
}