using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.NPC.Enemyes
{
  public class Tavros : Enemy
  {
	 /*
	  * Состояния:
	  * 0 - бездействие
	  * 1 - движение
	  * 2 - атака
	  * 3 - прыжок
	  * 
	  */


	 //private Game.NPC.Motions.AnimationWayPointsNawMeshMove _moveComponent;
	 //private Game.NPC.Motions.TavrosAttack _attackComponent;

	 protected override void Start()
	 {
		base.Start();

		//_moveComponent = GetComponent<Game.NPC.Motions.AnimationWayPointsNawMeshMove>();
		//_attackComponent = GetComponent<Game.NPC.Motions.TavrosAttack>();

		//_moveComponent.enabled = true;
		//_attackComponent.enabled = false;
	 }

	 protected void FixedUpdate()
	 {
		if (State == 2)
		  return;

		if (IsPlayerVisible())
		{
		  State = 2;
		  //_moveComponent.enabled = false;
		  //_attackComponent.enabled = true;
		}

		//if (IsPlayerVisible())
		//  Debug.Log("vidible");
		//else
		//  Debug.Log("in vidible");
	 }


  }
}