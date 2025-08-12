using UnityEngine;
using UnityEditor;
using System.Collections;
using RootMotion;
using it.Game.Player.Interactions;

namespace it.Game.Player.Interactions
{

	/*
	 * Custom inspector and scene view helpers for the InteractionTarget.
	 * */
	[CustomEditor(typeof(it.Game.Player.Interactions.InteractionTarget))]
	public class InteractionTargetInspector : Editor {

		private it.Game.Player.Interactions.InteractionTarget script { get { return target as it.Game.Player.Interactions.InteractionTarget; }}

		private const string twistAxisLabel = " Twist Axis";
		private const float size = 0.005f;
		private static Color targetColor = new Color(0.2f, 1f, 0.5f);
		private static Color pivotColor = new Color(0.2f, 0.5f, 1f);

		void OnSceneGUI() {
		UnityEditor.Handles.color = targetColor;

			Inspector.SphereCap(0, script.transform.position, Quaternion.identity, size);

			DrawChildrenRecursive(script.transform);

			if (script.pivot != null) {
		  UnityEditor.Handles.color = pivotColor;
				GUI.color = pivotColor;

				Inspector.SphereCap(0, script.pivot.position, Quaternion.identity, size);

				Vector3 twistAxisWorld = script.pivot.rotation * script.twistAxis.normalized * size * 40;
		  UnityEditor.Handles.DrawLine(script.pivot.position, script.pivot.position + twistAxisWorld);
				Inspector.SphereCap(0, script.pivot.position + twistAxisWorld, Quaternion.identity, size);

				Inspector.CircleCap(0, script.pivot.position, Quaternion.LookRotation(twistAxisWorld), size * 20);
		  UnityEditor.Handles.Label(script.pivot.position + twistAxisWorld, twistAxisLabel);
			}

		UnityEditor.Handles.color = Color.white;
			GUI.color = Color.white;
		}

		private void DrawChildrenRecursive(Transform t) {
			for (int i = 0; i < t.childCount; i++) {

		  UnityEditor.Handles.DrawLine(t.position, t.GetChild(i).position);
				Inspector.SphereCap(0, t.GetChild(i).position, Quaternion.identity, size);

				DrawChildrenRecursive(t.GetChild(i));
			}
		}
	}
}
