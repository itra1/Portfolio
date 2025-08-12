using UnityEngine;
using it.Game.Utils;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Environment.All.Animals
{
  [RequireComponent(typeof(ToGroundHeight))]
  public class AnimalSpaner : UUIDBase
  {
	 [SerializeField]
	 private bool _isEnable = true;
	 public bool IsEnable { get => _isEnable; set => _isEnable = value; }

	 [SerializeField]
	 private float _radius = 80;
	 public float Radius { get => _radius; }

	 [SerializeField]
	 private bool _randomScale;
	 public bool RandomScale { get => _randomScale; }

	 [SerializeField]
	 private FloatSpan _scaleSpan;
	 public FloatSpan ScaleSpan { get => _scaleSpan; set => _scaleSpan = value; }

	 [SerializeField]
	 private Vector3 _rotation;
	 public Vector3 Rotation { get => _rotation; set => _rotation = value; }

	 [SerializeField]
	 private bool _randomRotation;
	 public bool RandomRotation { get => _randomRotation; set => _randomRotation = value; }

	 [SerializeField]
	 private string _animaUuid;
	 public string AnimaUuid { get => _animaUuid; set => _animaUuid = value; }

	 private string _animalTitle;
	 public string AnimalTitle { get => _animalTitle; set => _animalTitle = value; }

	 private Transform _parent;
	 public Transform Parent
	 {
		get
		{
		  return _parent == null ? transform : _parent;
		}
		set => _parent = value;
	 }


	 [System.Serializable]
	 public struct FloatSpan
	 {
		public float min;
		public float max;

		public float Random => UnityEngine.Random.Range(min, max);
	 }

	 public void Rename()
	 {
		gameObject.name = "AnimalSpaner - " + AnimalTitle;
	 }

#if UNITY_EDITOR

	 private void OnDrawGizmosSelected()
	 {
		UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, _radius);

	 }

#endif
  }
}