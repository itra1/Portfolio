using UnityEngine;

namespace Scripts.Players {
	public class PlayerMove : MonoBehaviour {
		[SerializeField] private float _gravity;
		[SerializeField] private float _maxDownGravity;
		[SerializeField] private float _collisionUpImpuls;

		private Vector3 _velocity;
		private Rigidbody _rb;
		private float _lastImpuls;

		private void Awake() {
			if (_rb == null)
				_rb = GetComponent<Rigidbody>();
		}

		public void OnEnable() {
			if (_rb == null)
				_rb = GetComponent<Rigidbody>();
			_rb.velocity = Vector3.zero;
			_rb.isKinematic = false;
		}

		public void OnDisable() {
			_rb.velocity = Vector3.zero;
			_rb.isKinematic = true;
		}

		public void Clear() {
			_rb.velocity = Vector3.zero;
			_rb.isKinematic = true;
		}

		private void FixedUpdate() {
			var downVector = _rb.velocity.y;
			downVector += _gravity * Time.fixedDeltaTime;
			downVector = Mathf.Max(downVector, _maxDownGravity);
			_rb.velocity = Vector3.up * downVector;
		}

		private void OnCollisionEnter(Collision collision) {
			if (Time.time - _lastImpuls < 0.1f)
				return;
			_lastImpuls = Time.time;
			_rb.velocity = Vector3.zero;
			_rb.AddForce(Vector3.up * _collisionUpImpuls, ForceMode.Impulse);
		}

	}
}
