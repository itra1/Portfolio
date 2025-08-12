using UnityEngine;

namespace Scripts.Players
{
	public class PlayerMeshRotator :MonoBehaviour
	{
		[SerializeField] private float _minRotate;
		[SerializeField] private float _maxRotate;
		private Vector3 _rotation;

		public void OnEnable()
		{
			_rotation = Vector3.zero;
			transform.rotation = Quaternion.identity;
		}

		public void OnDisable()
		{
			_rotation = Vector3.zero;
		}

		public void NewRotate()
		{
			_rotation = new Vector3(UnityEngine.Random.Range(_minRotate, _maxRotate), UnityEngine.Random.Range(_minRotate, _maxRotate), UnityEngine.Random.Range(_minRotate, _maxRotate));
		}

		private void Update()
		{
			transform.Rotate(_rotation * Time.deltaTime);
		}

	}
}
