using UnityEngine;
using System.Collections;
using Aura2API;
using it.Game.Player;

namespace it.Game.Environment.Level5.AmbientLights
{
  public class AmbientLight : Environment
  {
	 [SerializeField]
	 private GameObject particle;

	 public void StartMove()
	 {
		State = 1;
		Save();
	 }

	 public void StopMove()
	 {
		State = 0;
		Save();
	 }

	 private void Update()
	 {
		if(State == 1)
		{
		  if (PlayerBehaviour.Instance != null)
			 particle.transform.position = PlayerBehaviour.Instance.transform.position;
		}
	 }

  }
}