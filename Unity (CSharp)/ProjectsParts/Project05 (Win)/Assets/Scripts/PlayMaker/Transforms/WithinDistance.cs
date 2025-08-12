using UnityEngine;
using HutongGames.PlayMaker;
using System.Collections.Generic;

namespace it.Game.PlayMaker.Transforms
{
  [ActionCategory("Move")]
  public class WithinDistance : FsmStateAction
  {
	 [RequiredField]
	 public FsmOwnerDefault gameObject;
	 [RequiredField]
	 public FsmGameObject targetObject;
	 public bool usePhysics2D;
	 public FsmString targetTag;
	 [UIHint(UIHint.Layer)]
	 public FsmInt[] objectLayerMask;
	 public FsmFloat magnitude = 5;
	 public FsmBool lineOfSight;
	 [UIHint(UIHint.Layer)]
	 public FsmInt[] ignoreLayerMask;
	 public FsmVector3 offset;
	 public FsmVector3 targetOffset;
	 public FsmGameObject returnedObject;

	 public FsmBool _everyFrame = true;

	 public FsmEvent onInDistance;
	 public FsmEvent onOutDistance;

	 private List<GameObject> objects;
	 // distance * distance, optimization so we don't have to take the square root
	 private float sqrMagnitude;

	 private GameObject _go;

	 public override void Awake()
	 {
		_go = Fsm.GetOwnerDefaultTarget(gameObject);

	 }

	 public override void OnEnter()
	 {
		sqrMagnitude = magnitude.Value * magnitude.Value;

		if (objects != null)
		{
		  objects.Clear();
		}
		else
		{
		  objects = new List<GameObject>();
		}

		if (targetObject.Value == null)
		{
		  if (!string.IsNullOrEmpty(targetTag.Value))
		  {
			 var gameObjects = GameObject.FindGameObjectsWithTag(targetTag.Value);
			 for (int i = 0; i < gameObjects.Length; ++i)
			 {
				objects.Add(gameObjects[i]);
			 }
		  }
		  else
		  {
			 var colliders = Physics.OverlapSphere(_go.transform.position, magnitude.Value, ActionHelpers.LayerArrayToLayerMask(objectLayerMask, false));
			 for (int i = 0; i < colliders.Length; ++i)
			 {
				objects.Add(colliders[i].gameObject);
			 }
		  }
		}
		else
		{
		  objects.Add(targetObject.Value);
		}


		Check();
	 }

	 public override void OnUpdate()
	 {
		if(_everyFrame.Value)
		  Check();
	 }

	 private void Check()
	 {
		if (CheckDistance())
		  Fsm.Event(onInDistance);
		else
		  Fsm.Event(onOutDistance);
	 }

	 private bool CheckDistance()
	 {
		Vector3 direction;

		for (int i = 0; i < objects.Count; ++i)
		{
		  if (objects[i] == null)
		  {
			 continue;
		  }
		  direction = objects[i].transform.position - (_go.transform.position + offset.Value);

		  if (Vector3.SqrMagnitude(direction) < sqrMagnitude)
		  {
			 if (lineOfSight.Value)
			 {
				if (LineOfSight(_go.transform, offset.Value, objects[i], targetOffset.Value, usePhysics2D, ActionHelpers.LayerArrayToLayerMask(ignoreLayerMask, true)))
				{
				  returnedObject.Value = objects[i];
				  return true;
				}
			 }
			 else
			 {
				returnedObject.Value = objects[i];
				return true;
			 }
		  }
		}

		return false;
	 }

	 public static GameObject LineOfSight(Transform transform, Vector3 positionOffset, GameObject targetObject, Vector3 targetOffset, bool usePhysics2D, int ignoreLayerMask)
	 {
		if (usePhysics2D)
		{
		  RaycastHit2D hit;
		  if ((hit = Physics2D.Linecast(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), ignoreLayerMask)))
		  {
			 if (hit.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(hit.transform))
			 {
				return targetObject;
			 }
		  }
		}
		else
		{
		  RaycastHit hit;
		  if (Physics.Linecast(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), out hit, ignoreLayerMask))
		  {
			 if (hit.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(hit.transform))
			 {
				return targetObject;
			 }
		  }
		}
		return null;
	 }

	 public override void Reset()
	 {
		usePhysics2D = false;
		targetObject = null;
		targetTag = string.Empty;
		objectLayerMask = null;
		magnitude = 5;
		lineOfSight = true;
		ignoreLayerMask = null;
		offset = Vector3.zero;
		targetOffset = Vector3.zero;
	 }

  }
}