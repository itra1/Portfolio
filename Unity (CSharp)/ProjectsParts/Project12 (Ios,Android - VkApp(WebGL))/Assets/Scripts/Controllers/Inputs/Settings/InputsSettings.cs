using UnityEngine;

namespace Game.Scripts.Controllers.Inputs.Settings
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "InputsSettings", menuName = "Settings/Inputs/InputsSettings")]
	public class InputsSettings : ScriptableObject
	{
		[SerializeField] private GameObject _pointPrefab;
		[SerializeField] private SimpleTrackInput[] _trackInput;
		[SerializeField] private SimpleTrackInput _swipeLeft = new(KeyCode.LeftArrow);
		[SerializeField] private SimpleTrackInput _swipeRight = new(KeyCode.RightArrow);
		[SerializeField] private SimpleTrackInput _swipeUp = new(KeyCode.UpArrow);
		[SerializeField] private SimpleTrackInput _swipeDown = new(KeyCode.DownArrow);
		[SerializeField] private LayerMask _touchInputMask;
		[SerializeField] private bool _2DTouchCollider = false;
		[SerializeField] private float _swipeThreshold = 20;
		[SerializeField] private bool _detectSwipeOnlyAfterRelease = false;
		[SerializeField] private bool _disableKeyInputInEditor = true;
		[SerializeField] private bool _disableTouchInputInEditor = true;
		[SerializeField] private bool _disableMouseInputInEditor = false;

		public GameObject PointPrefab => _pointPrefab;
		public SimpleTrackInput[] TrackInput => _trackInput;
		public SimpleTrackInput SwipeLeft => _swipeLeft;
		public SimpleTrackInput SwipeRight => _swipeRight;
		public SimpleTrackInput SwipeUp => _swipeUp;
		public SimpleTrackInput SwipeDown => _swipeDown;
		public LayerMask TouchInputMask => _touchInputMask;
		public float SwipeThreshold => _swipeThreshold;
		public bool DetectSwipeOnlyAfterRelease => _detectSwipeOnlyAfterRelease;
		public bool DisableKeyInputInEditor => _disableKeyInputInEditor;
		public bool DisableTouchInputInEditor => _disableTouchInputInEditor;
		public bool DisableMouseInputInEditor => _disableMouseInputInEditor;
		public bool TouchCollider2D => _2DTouchCollider;
	}
}
