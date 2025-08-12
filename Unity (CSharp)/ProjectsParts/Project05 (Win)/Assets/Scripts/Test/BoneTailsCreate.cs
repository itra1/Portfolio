using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneTailsCreate : MonoBehaviour
{
  [ContextMenu("Create")]
  public void Create()
  {
	 com.ootii.Actors.BoneControllers.BoneController bb 
		= GetComponent<com.ootii.Actors.BoneControllers.BoneController>();

	 var motors = new List<com.ootii.Actors.BoneControllers.BoneControllerMotor>(bb.Motors);

	 for(int i = 0; i < motors.Count; i++)
	 {
		var tail = new it.Game.Player.BoneControllers.TailsMotor();
		tail.Bones = motors[i].Bones;
		bb.Motors.Add(tail);
	 }

  }

  [ContextMenu("Clear")]
  public void Clear()
  {

	 com.ootii.Actors.BoneControllers.BoneController bb
		= GetComponent<com.ootii.Actors.BoneControllers.BoneController>();

	 bb.Motors.Clear();
  }

}
