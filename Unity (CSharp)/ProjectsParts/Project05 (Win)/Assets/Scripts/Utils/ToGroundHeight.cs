using UnityEngine;
using System.Collections;
using Utilites.Geometry;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Utils
{

#if UNITY_EDITOR

  [CustomEditor(typeof(ToGroundHeight))]
  [CanEditMultipleObjects]
  public class ToGroundHeightEditor : Editor
  {
	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		if (GUILayout.Button("To Ground"))
		{
		  for(int i = 0; i < targets.Length; i++)
		  {

			 ((ToGroundHeight)targets[i]).SetGround();
		  }

		}
		if (GUILayout.Button("Get Current Height"))
		{
		  for (int i = 0; i < targets.Length; i++)
		  {

			 ((ToGroundHeight)targets[i]).GetCurrentHeight();
		  }

		}

	 }
  }

#endif

  /// <summary>
  /// Установка обьевта на поверхность
  /// </summary>
  public class ToGroundHeight : MonoBehaviourBase
  {
	 /// <summary>
	 /// относительная (учитывается наклон обхекта)
	 /// </summary>
	 [Tooltip("Относительно направления объекта")]
	 [SerializeField]
	 private bool _relativeAngle = false;

	 [SerializeField]
	 private float _distantionCheck = 100;

	 [SerializeField]
	 private float _offsetY = 0;

	 /// <summary>
	 /// Установка на коллайдер
	 /// </summary>
	 public void SetGround()
	 {
		Vector3 startPoint = transform.position;
		RaycastHit[] hits;
		int count = RaycastExt.SafeRaycastAll(transform.position + Vector3.up * _distantionCheck / 3, Vector3.down, out hits, _distantionCheck * 1.3f, ProjectSettings.GroundAndClimbLayerMaks, transform);

		if (count <= 0)
		  return;

		Vector3 positionTarget = hits[0].point;
		positionTarget.y += _offsetY;

		transform.position = positionTarget;

	 }

	 public void GetCurrentHeight()
	 {

		Vector3 startPoint = transform.position;
		RaycastHit[] hits;
		int count = RaycastExt.SafeRaycastAll(transform.position + Vector3.up * _distantionCheck / 3, Vector3.down, out hits, _distantionCheck * 1.3f, ProjectSettings.GroundAndClimbLayerMaks, transform);

		if (count <= 0)
		  return;

		_offsetY = startPoint.y - hits[0].point.y;
	 }
  }
}