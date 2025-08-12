using UnityEngine;

namespace Game.Providers.Ui.Animations {
	[ExecuteInEditMode]
	public class Rotate :MonoBehaviour {
		[SerializeField] private RangeFloat _rangeSpeed;

		private RectTransform _rectTransform;
		private float _currentSpeed;
		public void Awake() {
			_rectTransform = GetComponent<RectTransform>();
		}

		public void OnEnable() {
			_rectTransform.eulerAngles = new(0, 0, Random.Range(0, 380f));
			_currentSpeed = _rangeSpeed.Random;
		}

		public void Update() {
			_rectTransform.eulerAngles = new(0, 0, _rectTransform.eulerAngles.z + _currentSpeed * Time.deltaTime);
		}
	}
}
