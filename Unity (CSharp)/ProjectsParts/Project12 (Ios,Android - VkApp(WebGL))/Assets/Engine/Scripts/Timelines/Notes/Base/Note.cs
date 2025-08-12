using Engine.Engine.Scripts.Managers.Interfaces;
using Engine.Engine.Scripts.Settings.Common;
using Engine.Scripts.Inputs;
using Engine.Scripts.Managers;
using Engine.Scripts.Settings.Common;
using Engine.Scripts.Timelines.Notes.Common;
using Engine.Scripts.Timelines.Notes.Factorys;
using Engine.Scripts.Timelines.Playables;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Engine.Scripts.Timelines.Notes.Base
{
	/// <summary>
	/// The states for the note.
	/// </summary>
	public enum ActiveState
	{
		Disabled, // When the Note has not been Initialized yet
		PreActive,  // Between the note being intialized and the active state
		Active,   // While the note is active
		PostActive  // While the note has been deactivated but not reinitialized.
	}

	/// <summary>
	/// The base class for the note component.
	/// </summary>
	public abstract class Note : MonoBehaviour
	{
		[HideInInspector] public UnityEvent<NoteTriggerEventData> OnNoteTriggerEvent { get; set; } = new();
		[HideInInspector] public UnityEvent<Note> OnInitialize { get; set; } = new();
		[HideInInspector] public UnityEvent<Note> OnReset { get; set; } = new();
		[HideInInspector] public UnityEvent<Note> OnActivate { get; set; } = new();
		[HideInInspector] public UnityEvent<Note> OnDeactivate { get; set; } = new();
		[HideInInspector] public UnityEvent OnRemoveVisible { get; set; } = new();

		[SerializeField] protected bool _updateWithTimeline = true;
		[SerializeField] protected bool _activateWithClip = false;
		[SerializeField] protected bool _orientToTrack = true;
		[SerializeField] private SpriteRenderer _texture;
		[SerializeField] protected SpriteRenderer _perfect;

		protected RhythmClipData _rhythmClipData;
		protected ActiveState _activeState;
		protected bool _deactivated;
		protected bool _isTriggered;
		protected double _actualInitializeTime;
		protected double _actualActivateTime;
		protected NoteTriggerEventData _noteTriggerEventData;
		private INoteAccuracy _perfectAccuracy;
		protected IDspTime _dspTime;
		protected float _noteSpeed;

		private INoteFactory _noteFactory;
		protected NoteGraphic _noteGraphic;

		public abstract string Type { get; }

		public ActiveState ActiveState => _activeState;
		public bool IsTriggered => _isTriggered;
		public double TrueInitializeTime => _rhythmClipData.ClipStart - RhythmClipData.RhythmDirector.SpawnTimeRange.Max;
		public double TrueActivateTime => _rhythmClipData.ClipStart;
		public double ActualInitializeTime => _actualInitializeTime;
		public double ActualActivateTime => _actualActivateTime;
		public double TimeFromActivate => CurrentTime - _rhythmClipData.ClipStart;
		public double TimeFromDeactivate => CurrentTime - _rhythmClipData.ClipEnd;
		public RhythmClipData RhythmClipData => _rhythmClipData;
		public double CurrentTime =>
			_updateWithTimeline || Application.isPlaying == false
				? _rhythmClipData.RhythmDirector.PlayableDirector.time
				: _dspTime.AdaptiveTime - _rhythmClipData.RhythmDirector.DspSongStartTime;

		private NoteTriggerEventData NoteTriggerEventData
		{
			get
			{
				_noteTriggerEventData ??= new()
				{
					Note = this
				};
				return _noteTriggerEventData;
			}
		}

		public SpriteRenderer Texture => _texture;

		public float NoteSpeed => _noteSpeed;

		[Inject]
		private void Constructor(
			INoteAccuracyGet noteAccuracy,
			IDspTime dspTime,
			INoteFactory noteFactory
		)
		{
			_dspTime = dspTime;
			_noteFactory = noteFactory;

			_perfectAccuracy = noteAccuracy.GetPerfectAccuracy();
		}

		public void SetGraphic(NoteGraphic noteGraphic)
		{
			_noteGraphic = noteGraphic;

			_texture.sprite = Type switch
			{
				NoteType.SwipeLeft or NoteType.SwipeRight or NoteType.SwipeUp or NoteType.SwipeDown => _noteGraphic.SwipeNoteSprite,
				NoteType.Tap or NoteType.Hold or _ => _noteGraphic.TapNoteSprite
			};
			_perfect.sprite = _noteGraphic.PerfectNoteSprite;
		}

		public void SetIsSpecialPoint()
		{
			_texture.sprite = _noteGraphic.SpecialNoteSprite;
		}

		public void SetIsFinalPoint()
		{
			_texture.sprite = _noteGraphic.FinalNoteSprite;
		}

		/// <summary>
		/// Create a cache the event data on awake.
		/// </summary>
		protected virtual void Awake()
		{
			_noteTriggerEventData ??= new()
			{
				Note = this
			};
		}

		protected virtual void Start() { }

		protected virtual void OnEnable()
		{
			SetPerfect(false);
		}

		protected virtual void OnDisable()
		{
			OnRemoveVisible?.Invoke();
			ClearListeners();
		}

		protected virtual double TimeDifferencePercentage()
		{
			var perfectTime = _rhythmClipData.RealDuration / 2f;
			var timeDifference = TimeFromActivate - perfectTime;
			return Mathf.Abs((float) (100f * timeDifference)) / perfectTime;
		}

		public void ClearListeners()
		{
			OnNoteTriggerEvent.RemoveAllListeners();
			OnInitialize.RemoveAllListeners();
			OnReset.RemoveAllListeners();
			OnActivate.RemoveAllListeners();
			OnDeactivate.RemoveAllListeners();
			OnRemoveVisible.RemoveAllListeners();
		}

		protected void SetPerfect(bool isPerfect)
		{
			if (_perfect != null)
				_perfect.gameObject.SetActive(isPerfect);
		}

		/// <summary>
		/// Initialize the note using the rhythm clip data.
		/// </summary>
		/// <param name="rhythmClipData">The rhythm clip data.</param>
		public virtual void Initialize(RhythmClipData rhythmClipData)
		{
			_rhythmClipData = rhythmClipData;

			SetNormalEventType();
			_noteSpeed = _rhythmClipData.RhythmDirector.NoteSpeed;

			if (rhythmClipData.TrackObject == null)
			{
				Debug.LogWarning("The Track Object cannot be null", gameObject);
				return;
			}

			if (_orientToTrack)
			{
				transform.rotation = rhythmClipData.TrackObject.StartPoint.rotation;
			}
			else
			{
				transform.right = Vector3.left;
			}

			_isTriggered = false;
			_activeState = ActiveState.PreActive;

			_actualInitializeTime = CurrentTime;
			_actualActivateTime = -1;

			RhythmClipData.TrackObject.SetVisibleNote(this);

			InvokeOnInitialize();
		}

		public void SetNormalEventType()
		{
			NoteTriggerEventData.EventType = NoteEventType.Normal;
		}

		/// <summary>
		/// Invoke the On Initialize event.
		/// </summary>
		protected virtual void InvokeOnInitialize()
		{
			OnInitialize?.Invoke(this);
		}

		/// <summary>
		/// Reset when the note is returned to the pool.
		/// </summary>
		public virtual void Reset()
		{
			_activeState = ActiveState.Disabled;
			InvokeOnReset();
		}

		/// <summary>
		/// Invoked the on reset event.
		/// </summary>
		protected virtual void InvokeOnReset()
		{
			OnReset?.Invoke(this);
		}

		/// <summary>
		/// The clip started.
		/// </summary>
		public virtual void OnClipStart()
		{
			if (_activateWithClip)
			{
				ActivateNote();
			}
		}

		/// <summary>
		/// The clip stopped.
		/// </summary>
		public virtual void OnClipStop()
		{
			if (_activateWithClip)
			{
				DeactivateNote();
			}
		}

		/// <summary>
		/// The note needs to be activated as it is within range of being triggered.
		/// This usually happens when the clip starts.
		/// </summary>
		protected virtual void ActivateNote()
		{
			if (_noteTriggerEventData.EventType != NoteEventType.Normal)
				return;
			_activeState = ActiveState.Active;

			RhythmClipData.TrackObject.SetActiveNote(this);
			RhythmClipData.TrackObject.RemoveVisibleNote(this);

			_actualActivateTime = CurrentTime;
			InvokeOnActivate();
		}

		/// <summary>
		/// Invoke the on activate event.
		/// </summary>
		protected virtual void InvokeOnActivate()
		{
			OnActivate?.Invoke(this);
		}

		/// <summary>
		/// The note was deactivated.
		/// </summary>
		protected virtual void DeactivateNote()
		{
			if (_noteTriggerEventData.EventType != NoteEventType.Normal)
				return;
			_activeState = ActiveState.PostActive;
			RhythmClipData.TrackObject.RemoveActiveNote(this);

			InvokeOnDeactivate();

			if (Application.isPlaying)
				_noteFactory.FreeInstance(this);
		}

		/// <summary>
		/// Invoke the on deactivate event.
		/// </summary>
		protected virtual void InvokeOnDeactivate()
		{
			OnDeactivate?.Invoke(this);
		}

		/// <summary>
		/// Trigger an input on the note.
		/// </summary>
		/// <param name="inputEventData">The input event data.</param>
		public abstract void OnTriggerInput(InputEventData inputEventData);

		/// <summary>
		/// Invoke note trigger event.
		/// </summary>
		protected virtual void InvokeNoteTriggerEvent()
		{
			OnNoteTriggerEvent?.Invoke(_noteTriggerEventData);
		}

		/// <summary>
		/// Trigger a note trigger event with the offset.
		/// </summary>
		/// <param name="eventData">The input event data.</param>
		/// <param name="dspTimeDiff">The offset from a perfect hit.</param>
		/// <param name="dspTimeDiffPerc">The offset from a perfect hit in percentage.</param>
		protected virtual void InvokeNoteTriggerEvent(InputEventData eventData, double dspTimeDiff, float dspTimeDiffPerc)
		{
			_noteTriggerEventData.SetTriggerData(eventData, dspTimeDiff, dspTimeDiffPerc, _dspTime.AdaptiveTime);
			InvokeNoteTriggerEvent();
		}

		/// <summary>
		/// The timeline update, updates every frame and in edit mode too.
		/// </summary>
		/// <param name="globalClipStartTime">The offset to the clip start time.</param>
		/// <param name="globalClipEndTime">The offset to the clip stop time</param>
		public virtual void TimelineUpdate(double globalClipStartTime, double globalClipEndTime)
		{
			if (!_updateWithTimeline && !Application.isPlaying)
				return;

			if (!_activateWithClip)
			{
				if (_activeState == ActiveState.Active && globalClipEndTime >= 0)
				{
					DeactivateNote();
				}
				else if (_activeState == ActiveState.PreActive && globalClipStartTime >= 0)
				{
					ActivateNote();
				}
			}

			HybridUpdate(globalClipStartTime, globalClipEndTime);
			PerfectCheck();
		}

		/// <summary>
		/// Default update can be used instead of timeline update to sync with DSP adaptive time.
		/// </summary>
		protected virtual void Update()
		{
			if (_updateWithTimeline)
				return;

			if (!_activateWithClip)
			{
				if (_activeState == ActiveState.Active && TimeFromDeactivate >= 0)
				{
					DeactivateNote();
				}
				else if (_activeState == ActiveState.PreActive && TimeFromActivate >= 0)
				{
					ActivateNote();
				}
			}

			HybridUpdate(TimeFromActivate, TimeFromDeactivate);

			PerfectCheck();
		}

		private void PerfectCheck()
		{
			_perfect.gameObject.SetActive(_perfectAccuracy != null && _perfectAccuracy.IsActualPercentageTheshold((float) TimeDifferencePercentage()));
		}

		/// <summary>
		/// Invoke the note trigger missed event.
		/// </summary>
		protected virtual void InvokeNoteTriggerEventMiss()
		{
			if (_noteTriggerEventData.EventType != NoteEventType.Normal)
				return;

			_noteTriggerEventData.SetLoss(_dspTime.AdaptiveTime, NoteEventType.Miss);
			InvokeNoteTriggerEvent();
		}

		public void DestroyLoss()
		{
			if (_noteTriggerEventData.EventType != NoteEventType.Normal)
				return;

			_noteTriggerEventData.SetLoss(_dspTime.AdaptiveTime, NoteEventType.Early);

			InvokeNoteTriggerEvent();
		}

		/// <summary>
		/// Hybrid update works both in play and edit mode.
		/// </summary>
		/// <param name="timeFromStart">The offset before the start.</param>
		/// <param name="timeFromEnd">The offset before the end.</param>
		protected abstract void HybridUpdate(double timeFromStart, double timeFromEnd);

	}
}