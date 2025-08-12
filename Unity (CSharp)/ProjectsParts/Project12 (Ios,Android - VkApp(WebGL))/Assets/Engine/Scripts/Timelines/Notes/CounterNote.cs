using Engine.Scripts.Inputs;
using Engine.Scripts.Timelines.Playables;
using Engine.Scripts.Timelines.Notes.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.Scripts.Timelines.Notes
{
	/// <summary>
	/// The Counter Note is pressed multiple times as fast as possible during a limited time.
	/// </summary>
	public class CounterNote : Note
	{
		[FormerlySerializedAs("m_TmpText")]
		[Tooltip("The Counter Text.")]
		[SerializeField] protected TMP_Text _counterText;

		private int _startCounter;
		private int _counter;

		public override string Type => NoteType.Counter;

		/// <summary>
		/// Initialize the Note.
		/// </summary>
		/// <param name="rhythmClipData">The rhythm clip data.</param>
		public override void Initialize(RhythmClipData rhythmClipData)
		{
			base.Initialize(rhythmClipData);
			_startCounter = _rhythmClipData.ClipParameters.IntParameter;
			SetCounter(_startCounter);
		}

		/// <summary>
		/// Set the number of times the note must be tapped
		/// </summary>
		/// <param name="counter">The amount of times to tap.</param>
		protected void SetCounter(int counter)
		{
			_counter = counter;
			_counterText.text = _counter.ToString();
		}

		/// <summary>
		/// The note was deactivated.
		/// </summary>
		protected override void DeactivateNote()
		{
			base.DeactivateNote();

			if (Application.isPlaying == false)
				return;

			if (_isTriggered == false)
			{
				InvokeNoteTriggerEventMiss();
			}
			else if (_counter > 0)
			{
				var percentage = 100 * (_counter / (float) _startCounter);
				InvokeNoteTriggerEvent(null, _rhythmClipData.RhythmDirector.Crochet, percentage);
			}
		}

		/// <summary>
		/// Trigger an input on the note. Detect taps.
		/// </summary>
		/// <param name="inputEventData">The input event data.</param>
		public override void OnTriggerInput(InputEventData inputEventData)
		{
			if (!inputEventData.Tap)
				return;

			_isTriggered = true;

			SetCounter(_counter - 1);
			if (_counter <= 0)
			{
				gameObject.SetActive(false);
				InvokeNoteTriggerEvent(inputEventData, 0, 0);
				RhythmClipData.TrackObject.RemoveActiveNote(this);
			}
		}

		/// <summary>
		/// Hybrid update works both in play and edit mode.
		/// </summary>
		/// <param name="timeFromStart">The offset before the start.</param>
		/// <param name="timeFromEnd">The offset before the end.</param>
		protected override void HybridUpdate(double timeFromStart, double timeFromEnd)
		{
			var deltaTStart = (float) (timeFromStart - _rhythmClipData.RhythmDirector.HalfCrochet);
			var deltaTEnd = (float) (timeFromEnd + _rhythmClipData.RhythmDirector.HalfCrochet);

			Vector3 newPosition;
			if (timeFromStart < _rhythmClipData.RhythmDirector.HalfCrochet)
			{
				//Move
				newPosition = GetNotePosition(deltaTStart);
			}
			else if (timeFromEnd < -_rhythmClipData.RhythmDirector.HalfCrochet)
			{
				//Wait
				newPosition = GetNotePosition(0);
			}
			else
			{
				//Move Again
				newPosition = GetNotePosition(deltaTEnd);
			}

			transform.position = newPosition;
		}

		/// <summary>
		/// Get the position of the Note for the delta time.
		/// </summary>
		/// <param name="deltaT">The delta time.</param>
		/// <returns>The position of the note.</returns>
		protected Vector3 GetNotePosition(float deltaT)
		{
			var direction = RhythmClipData.TrackObject.GetNoteDirection(deltaT);
			var distance = deltaT * _noteSpeed;
			var targetPosition = _rhythmClipData.TrackObject.EndPoint.position;

			return targetPosition + (direction * distance);
		}
	}
}