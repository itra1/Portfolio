using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Game.Player.Interactions
{
#if UNITY_EDITOR

  [CustomEditor(typeof(InteractionObjectTemplate))]
  public class InteractionObjectTemplateEditor: Editor
  {
	 public override void OnInspectorGUI()
	 {

		if(GUILayout.Button("Set curves"))
		{
		  ((InteractionObjectTemplate)target).SetCurves();
		}
		base.OnInspectorGUI();

	 }
  }

#endif

  public class InteractionObjectTemplate : MonoBehaviourBase
  {
	 public InteractionObject interObject;
	 public List<InteractionCurvesData> _curveData;
	 public bool checkHeight = true;

	 [System.Serializable]
	 public struct InteractionCurvesData
	 {
		public HeightType type;
		public InteractionObject.WeightCurve[] weightCurves;
		public InteractionObject.Multiplier[] multipliers;
	 }

	 public void SetCurves()
	 {
		if(interObject == null)
		interObject = GetComponent<InteractionObject>();

		InteractionTarget target = interObject.GetComponentInChildren<InteractionTarget>();
		HeightType targetType = HeightType.low;
		if (checkHeight)
		{

		  float deltaHeight = target.transform.localPosition.y;

		  if (deltaHeight <= 0.6f)
			 targetType = HeightType.low;
		  else if (deltaHeight <= 1.2f)
			 targetType = HeightType.middle;
		  else
			 targetType = HeightType.hight;
		}
		else
		{
		  switch (interObject.InteractionVariant)
		  {
			 case InteractionObject.InteractionType.up:
				targetType = HeightType.hight;
				break;
			 case InteractionObject.InteractionType.middle:
				targetType = HeightType.middle;
				break;
			 case InteractionObject.InteractionType.down:
				targetType = HeightType.low;
				break;
			 default:
				return;
		  }
		}

		InteractionCurvesData dat = _curveData.Find(x => x.type == targetType);

		interObject.weightCurves = dat.weightCurves;
		interObject.multipliers = dat.multipliers;
	 }

	 public enum HeightType
	 {
		low,
		middle,
		hight
	 }
  }
}