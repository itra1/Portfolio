using UnityEngine;

namespace Scripts.GameItems.Platforms {
	[System.Serializable]
	public class PlatformElement : MonoBehaviour {

		[SerializeField][Uuid.UUID(true)] private string _uuid;
		[SerializeField] private bool _isDamage;

		public string Uuid { get => _uuid; set => _uuid = value; }
		public bool IsDamage { get => _isDamage; set => _isDamage = value; }

		[ContextMenu("ShowVectors")]
		public void ShowVectors() {
			Vector3 forceVector = -transform.up;

			var angle = Vector3.Angle(Vector3.back, forceVector);
			var dot = Vector3.Dot(Vector3.back, forceVector);
			var vectorSign = Vector3.SignedAngle(Vector3.back, forceVector, Vector3.up);
			Debug.Log(vectorSign + " : " + angle + " : " + dot);
		}

	}
}
