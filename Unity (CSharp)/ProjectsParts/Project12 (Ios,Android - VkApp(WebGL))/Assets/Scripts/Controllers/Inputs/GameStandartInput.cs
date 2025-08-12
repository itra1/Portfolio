using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Engine.Engine.Scripts.Managers.Interfaces;
using Engine.Engine.Scripts.Settings;
using Engine.Scripts.Inputs;
using Engine.Scripts.Managers;
using Game.Scripts.Controllers.Inputs.Settings;
using Game.Scripts.Controllers.Sessions.Common;
using Game.Shared;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers.Inputs
{
	/// <summary>
	/// A simple abstraction to allow either key or button input.
	/// </summary>
	[Serializable]
	public struct SimpleTrackInput
	{
		[SerializeField] private KeyCode m_Key;
		[SerializeField] private string m_Button;

		public SimpleTrackInput(KeyCode key, string button = null)
		{
			m_Key = key;
			m_Button = button;
		}

		public bool GetInputDown()
		{
			var input = false;

			if (m_Key != KeyCode.None)
			{
				input |= Input.GetKeyDown(m_Key);
			}

			if (!string.IsNullOrWhiteSpace(m_Button))
			{
				input |= Input.GetButtonDown(m_Button);
			}

			return input;
		}

		public bool GetInputUp()
		{
			var input = false;

			if (m_Key != KeyCode.None)
			{
				input |= Input.GetKeyUp(m_Key);
			}

			if (!string.IsNullOrWhiteSpace(m_Button))
			{
				input |= Input.GetButtonUp(m_Button);
			}

			return input;
		}

		public bool GetInput()
		{
			var input = false;

			if (m_Key != KeyCode.None)
			{
				input |= Input.GetKey(m_Key);
			}

			if (!string.IsNullOrWhiteSpace(m_Button))
			{
				input |= Input.GetButton(m_Button);
			}

			return input;
		}
	}

	/// <summary>
	/// Gets information from the RhythmDirector and from the input to processes notes.
	/// </summary>
	public class GameStandartInput : IInitializable, ITickable
	{
		public const int c_MouseTouchFingerID = -1;

		private List<GameObject> _touchPointInstances = new();
		private InputEventData[] _trackInputEventData;
		private Camera _camera;
		private Dictionary<int, Vector2> _touchBeganPosition;
		private Dictionary<int, Vector2> _touchEndedPosition;
		private Dictionary<int, int> _touchToTrackMap;
		private readonly InputsSettings _inputSettings;
		private IGamePlaying _gamePlaying;
		private IRhythmProcessor _rhythmProcessor;
		private ISession _session;
		private IPoolParent _poolParent;
		private IApplicationSettings _applicationSettings;

		private bool KeyInputEnable
		{
			get
			{
#if UNITY_EDITOR
				return !_inputSettings.DisableKeyInputInEditor;
#else
				return _applicationSettings != null ? !_applicationSettings.InputSettings.DisableKeyboard : false;
#endif
			}
		}
		private bool MouseInputEnable
		{
			get
			{
#if UNITY_EDITOR
				return !_inputSettings.DisableMouseInputInEditor;
#else
				return _applicationSettings != null ? !_applicationSettings.InputSettings.DisableMouse : false;
#endif
			}
		}
		private bool TouchInputEnable
		{
			get
			{
#if UNITY_EDITOR
				return !_inputSettings.DisableTouchInputInEditor;
#else
				return _applicationSettings != null ? !_applicationSettings.InputSettings.DisableTouch : false;
#endif
			}
		}

		public GameStandartInput(
		InputsSettings inputSettings,
			IGamePlaying gamePlaying,
			IRhythmProcessor rhythmProcessor,
			ISession session,
			IPoolParent poolParent,
			IApplicationSettingsController applicationSettingsController
		)
		{
			_inputSettings = inputSettings;
			_gamePlaying = gamePlaying;
			_rhythmProcessor = rhythmProcessor;
			_session = session;
			_poolParent = poolParent;
			applicationSettingsController.OnLoadSettings.AddListener((v) => _applicationSettings = v);
		}

		//[Inject]
		//public void Constructor(
		//	IGamePlaying gamePlaying,
		//	IRhythmProcessor rhythmProcessor,
		//	ISession session,
		//	IPoolParent poolParent
		//)
		//{
		//	_gamePlaying = gamePlaying;
		//	_rhythmProcessor = rhythmProcessor;
		//	_session = session;
		//	_poolParent = poolParent;
		//}

		//protected virtual void Awake()
		//{
		//}

		//public virtual void Update()
		//{
		//}

		public void Initialize()
		{
			_camera = Camera.main;

			_trackInputEventData = new InputEventData[_inputSettings.TrackInput.Length];
			for (int i = 0; i < _trackInputEventData.Length; i++)
			{
				_trackInputEventData[i] = new InputEventData(i, -1);
			}

			_touchBeganPosition = new Dictionary<int, Vector2>();
			_touchEndedPosition = new Dictionary<int, Vector2>();
			_touchToTrackMap = new Dictionary<int, int>();
		}

		public void Tick()
		{
			if (!_gamePlaying.IsPlaying)
				return;

			if (KeyInputEnable)
				TickKeyInput();
			if (TouchInputEnable)
				TickTouch();
			if (MouseInputEnable)
				TickMouseClick();
		}

		public virtual void TickKeyInput()
		{
			for (int i = 0; i < _inputSettings.TrackInput.Length; i++)
			{
				var input = _inputSettings.TrackInput[i];
				var trackInputEventData = _trackInputEventData[i];
				trackInputEventData.TouchAsSwipe = _applicationSettings.InputSettings.SingleTouchSwift;

				if (input.GetInputDown())
				{
					TriggerInput(trackInputEventData, 0);
				}

				if (input.GetInputUp())
				{
					TriggerInput(trackInputEventData, 1);
				}

				// Trigger swipe input if the swipe input is pressed while the track button is hold.
				if (input.GetInput())
				{
					if (_inputSettings.SwipeDown.GetInput())
					{
						TriggerInput(trackInputEventData, 2, Vector2.down);
					}
					if (_inputSettings.SwipeUp.GetInput())
					{
						TriggerInput(trackInputEventData, 2, Vector2.up);
					}
					if (_inputSettings.SwipeLeft.GetInput())
					{
						TriggerInput(trackInputEventData, 2, Vector2.left);
					}
					if (_inputSettings.SwipeRight.GetInput())
					{
						TriggerInput(trackInputEventData, 2, Vector2.right);
					}
				}
			}
		}

		public virtual void TickTouch()
		{
			for (int i = 0; i < Input.touches.Length; i++)
			{
				var touch = Input.touches[i];
				var inputPosition = touch.position;

				//previous touche can be released on another track
				if (_touchToTrackMap.TryGetValue(touch.fingerId, out var trackID) && trackID != -1)
				{

					var previousTrackInputEventData = _trackInputEventData[trackID];

					//Detects Swipe while finger is still moving
					//if (touch.phase == TouchPhase.Moved)
					//{
					//	InputMoved(touch.fingerId, previousTrackInputEventData, touch.position);
					//}

					if (touch.phase == TouchPhase.Ended)
					{
						InputReleased(touch.fingerId, previousTrackInputEventData, touch.position);
					}
				}
				else
				{
					trackID = -1;
				}

				var trackInputEventData = GetTrackInputEventData(inputPosition);

				if (trackInputEventData == null)
					continue;

				//The input event was already checked for this track
				if (trackID == trackInputEventData.TrackID)
					continue;

				if (touch.phase == TouchPhase.Began)
				{
					InputPressed(touch.fingerId, trackInputEventData, touch.position);
				}

				//Detects Swipe while finger is still moving
				//if (touch.phase == TouchPhase.Moved)
				//{
				//	InputMoved(touch.fingerId, trackInputEventData, touch.position);
				//}

				if (touch.phase == TouchPhase.Ended)
				{
					InputReleased(touch.fingerId, trackInputEventData, touch.position);
				}
			}
		}

		void CheckSwipe(InputEventData trackInputEventData, int fingerId)
		{
			if (!_touchEndedPosition.ContainsKey(fingerId) || !_touchBeganPosition.ContainsKey(fingerId))
				return;

			var direction = _touchEndedPosition[fingerId] - _touchBeganPosition[fingerId];
			var sqrDistance = direction.sqrMagnitude;

			if (sqrDistance > _inputSettings.SwipeThreshold)
				TriggerInput(trackInputEventData, 2, direction);
		}

		private void TickMouseClick()
		{

			if (Input.GetMouseButtonDown(0))
			{

				var inputPosition = Input.mousePosition;

				var trackInputEventData = GetTrackInputEventData(inputPosition);

				if (trackInputEventData == null)
					return;

				InputPressed(c_MouseTouchFingerID, trackInputEventData, inputPosition);
			}

			if (!_inputSettings.DetectSwipeOnlyAfterRelease && Input.GetMouseButton(0))
			{

				var inputPosition = Input.mousePosition;

				//Check for previous track
				if (_touchToTrackMap.TryGetValue(c_MouseTouchFingerID, out var trackID) && trackID != -1)
				{
					var previousTrackInputEventData = _trackInputEventData[trackID];
					InputMoved(c_MouseTouchFingerID, previousTrackInputEventData, inputPosition);
				}
				else
				{
					trackID = -1;
				}
				var trackInputEventData = GetTrackInputEventData(inputPosition);

				if (trackInputEventData == null || trackInputEventData.TrackID == trackID)
					return;

				InputMoved(c_MouseTouchFingerID, trackInputEventData, inputPosition);
			}

			if (Input.GetMouseButtonUp(0))
			{
				var inputPosition = Input.mousePosition;

				//Chceck for previous track
				if (_touchToTrackMap.TryGetValue(c_MouseTouchFingerID, out var trackID) && trackID != -1)
				{
					var previousTrackInputEventData = _trackInputEventData[trackID];
					InputReleased(c_MouseTouchFingerID, previousTrackInputEventData, inputPosition);
				}
				else
				{
					trackID = -1;
				}
				var trackInputEventData = GetTrackInputEventData(inputPosition);

				if (trackInputEventData == null || trackInputEventData.TrackID == trackID)
					return;

				InputReleased(c_MouseTouchFingerID, trackInputEventData, inputPosition);
			}
		}

		protected virtual void InputPressed(int fingerID, InputEventData trackInputEventData, Vector3 inputPosition)
		{
			TriggerInput(trackInputEventData, 0);
			_touchBeganPosition[fingerID] = inputPosition;
			_touchEndedPosition[fingerID] = inputPosition;
			_touchToTrackMap[fingerID] = trackInputEventData.TrackID;

			if (_session.TapVisible)
				_ = VisibleTouchPoint(Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 10)));
		}

		protected virtual void InputMoved(int fingerID, InputEventData previousTrackInputEventData, Vector3 inputPosition)
		{
			if (_inputSettings.DetectSwipeOnlyAfterRelease)
				return;

			_touchEndedPosition[fingerID] = inputPosition;
			CheckSwipe(previousTrackInputEventData, fingerID);
		}

		protected virtual void InputReleased(int fingerID, InputEventData previousTrackInputEventData, Vector3 inputPosition)
		{
			TriggerInput(previousTrackInputEventData, 1);
			_touchEndedPosition[fingerID] = inputPosition;
			_touchToTrackMap[fingerID] = -1;
			CheckSwipe(previousTrackInputEventData, fingerID);

			if (_session.TapVisible)
				_ = VisibleTouchPoint(Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 10)));
		}

		private async UniTask VisibleTouchPoint(Vector2 targetPosition)
		{
			var instance = _touchPointInstances.Find(x => !x.activeSelf);
			if (instance == null)
			{
				_inputSettings.PointPrefab.SetActive(false);
				instance = MonoBehaviour.Instantiate(_inputSettings.PointPrefab, _poolParent.PoolParent);
				_touchPointInstances.Add(instance);
			}

			instance.transform.position = targetPosition;
			instance.SetActive(true);
			await UniTask.Delay(500);
			instance.SetActive(false);
		}

		protected virtual InputEventData GetTrackInputEventData(Vector2 inputPosition)
		{
			var tackObjects = _rhythmProcessor.RhythmDirector.TrackObjects;

			var ray = _camera.ScreenPointToRay(inputPosition);

			if (_inputSettings.TouchCollider2D)
			{
				var hit = Physics2D.Raycast(ray.origin, ray.direction, 100, _inputSettings.TouchInputMask);

				if (hit.collider == null)
					return null;

				for (int i = 0; i < tackObjects.Length; i++)
				{
					if (hit.collider != tackObjects[i].TouchCollider2D)
						continue;

					return _trackInputEventData[i];
				}

				return null;
			}

			if (Physics.Raycast(ray, out var hitInfo, 100, _inputSettings.TouchInputMask) == false)
				return null;

			for (int i = 0; i < tackObjects.Length; i++)
			{
				if (hitInfo.collider != tackObjects[i].TouchCollider3D)
					continue;

				return _trackInputEventData[i];
			}

			return null;
		}

		protected virtual void TriggerInput(InputEventData trackInputEventData, int inputID, Vector2 direction)
		{
			trackInputEventData.InputID = inputID;
			trackInputEventData.Direction = direction;
			TriggerInput(trackInputEventData);
		}

		protected virtual void TriggerInput(InputEventData trackInputEventData, int inputID)
		{
			trackInputEventData.InputID = inputID;
			trackInputEventData.Direction = Vector2.zero;
			TriggerInput(trackInputEventData);
		}

		protected virtual void TriggerInput(InputEventData trackInputEventData)
		{
			_rhythmProcessor.TriggerInput(trackInputEventData);
		}
	}
}