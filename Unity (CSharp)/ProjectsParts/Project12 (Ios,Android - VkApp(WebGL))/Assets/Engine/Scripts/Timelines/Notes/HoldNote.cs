using Engine.Scripts.Inputs;
using Engine.Scripts.Timelines.Playables;
using Engine.Scripts.Timelines.Notes.Base;
using UnityEngine;

namespace Engine.Scripts.Timelines.Notes
{
	/// <summary>
	/// A hold note is a note that must be pressed at a start point and released at an end point.
	/// </summary>
	public class HoldNote : Note
	{
		[SerializeField] private Transform _startNote;
		[SerializeField] private Transform _startLineNote;
		[SerializeField] private Transform _startLineLightNote;
		[SerializeField] private Transform _endNote;
		[SerializeField] private LineRenderer _lineRenderer;
		[SerializeField] private LineRenderer _lineLightRenderer;
		[SerializeField] private Transform _linePointRenderer;
		[SerializeField] private bool _autoPerfectRelease;
		[SerializeField] private bool _removeNoteIfMissed = true;

		private bool _holding;
		private Color _startLineColor;
		private double _startHoldTimeOffset;

		public override string Type => NoteType.Hold;

		/// <summary>
		/// Initialize the note.
		/// </summary>
		/// <param name="rhythmClipData">The rhythm Clip Data.</param>
		public override void Initialize(RhythmClipData rhythmClipData)
		{
			base.Initialize(rhythmClipData);
			_holding = false;
			_lineRenderer.positionCount = 2;
			_lineLightRenderer.positionCount = 2;
			_startLineColor = _lineRenderer.startColor;
			_startHoldTimeOffset = 0;
			_lineRenderer.colorGradient = _noteGraphic.NoteHoldGradient;
		}

		/// <summary>
		/// Reset the note when it is returned to the pool.
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			_lineRenderer.startColor = _startLineColor;
		}

		/// <summary>
		/// Later update happens after update, the line must be updated after the notes have been moved.
		/// </summary>
		protected virtual void LateUpdate()
		{
			UpdateLinePositions();
		}

		/// <summary>
		/// The timeline update, updates every frame and in edit mode too.
		/// </summary>
		/// <param name="globalClipStartTime">The offset to the clip start time.</param>
		/// <param name="globalClipEndTime">The offset to the clip stop time</param>
		public override void TimelineUpdate(double globalClipStartTime, double globalClipEndTime)
		{
			base.TimelineUpdate(globalClipStartTime, globalClipEndTime);
			UpdateLinePositions();
		}

		/// <summary>
		/// Update the positions of the line renderer.
		/// </summary>
		private void UpdateLinePositions()
		{
			var startPoint = _startNote.transform.localPosition;
			var endPoint = _endNote.transform.localPosition;
			float height = endPoint.y - startPoint.y;

			_lineRenderer.positionCount = height > 0.6f ? 4 : 2;
			int index = -1;
			_lineRenderer.SetPosition(++index, startPoint);
			if (height > 0.6f)
			{
				_lineRenderer.SetPosition(++index, startPoint + new Vector3(0, 0.3f, 0));
				_lineRenderer.SetPosition(++index, endPoint + new Vector3(0, -0.3f, 0) - _lineRenderer.transform.localPosition);
			}
			_lineRenderer.SetPosition(++index, endPoint - _lineRenderer.transform.localPosition);

			_lineLightRenderer.positionCount = 2;
			index = -1;
			_lineLightRenderer.SetPosition(++index, startPoint);
			_lineLightRenderer.SetPosition(++index, endPoint - _lineLightRenderer.transform.localPosition);

			_linePointRenderer.localPosition = endPoint;
		}

		/// <summary>
		/// The note needs to be activated as it is within range of being triggered.
		/// This usually happens when the clip starts.
		/// </summary>
		protected override void ActivateNote()
		{
			base.ActivateNote();
		}

		/// <summary>
		/// The note was deactivated.
		/// </summary>
		protected override void DeactivateNote()
		{
			if (!_isTriggered)
			{
				InvokeNoteTriggerEventMiss();
			}

			base.DeactivateNote();

			if (!Application.isPlaying)
				return;
		}

		/// <summary>
		/// Trigger an input on the note. Detect both tap and release inputs.
		/// </summary>
		/// <param name="inputEventData">The input event data.</param>
		public override void OnTriggerInput(InputEventData inputEventData)
		{
			if (inputEventData.Tap)
			{
				_holding = true;

				//_startNote.position = _rhythmClipData.TrackObject.EndPoint.position;

				var perfectTime = _rhythmClipData.RhythmDirector.HalfCrochet;
				var timeDifference = TimeFromActivate - perfectTime;

				_startHoldTimeOffset = timeDifference;
			}

			if (_holding && inputEventData.Release)
			{

				//gameObject.SetActive(false);
				_isTriggered = true;

				var perfectTime = _rhythmClipData.RhythmDirector.HalfCrochet;
				var timeDifference = TimeFromDeactivate + perfectTime;

				var averageTotalTimeDifference = (_startHoldTimeOffset + timeDifference) / 2f;
				var timeDifferencePercentage = Mathf.Abs((float) (100f * averageTotalTimeDifference)) / perfectTime;

				InvokeNoteTriggerEvent(inputEventData, timeDifference, (float) timeDifferencePercentage);
				RhythmClipData.TrackObject.RemoveActiveNote(this);
				DeactivateNote();
			}
		}

		protected override double TimeDifferencePercentage()
		{
			var perfectTime = _rhythmClipData.RhythmDirector.HalfCrochet;
			var timeDifference = TimeFromDeactivate + perfectTime;

			var averageTotalTimeDifference = (_startHoldTimeOffset + timeDifference) / 2f;
			return Mathf.Abs((float) (100f * averageTotalTimeDifference)) / perfectTime;
		}

		/// <summary>
		/// Hybrid update works both in play and edit mode.
		/// </summary>
		/// <param name="timeFromStart">The offset before the start.</param>
		/// <param name="timeFromEnd">The offset before the end.</param>
		protected override void HybridUpdate(double timeFromStart, double timeFromEnd)
		{
			if (!Application.isPlaying && _startNote == null)
				return;

			if (Application.isPlaying && (_activeState == ActiveState.PostActive || _activeState == ActiveState.Disabled))
				return;

			var deltaTStart = (float) (timeFromStart - _rhythmClipData.RhythmDirector.HalfCrochet);
			var deltaTEnd = (float) (timeFromEnd + _rhythmClipData.RhythmDirector.HalfCrochet);

			if (_holding == false)
			{
				_startNote.position = GetNotePosition(deltaTStart);

				if (Application.isPlaying)
				{
					if (_removeNoteIfMissed && timeFromStart > _rhythmClipData.RhythmDirector.Crochet)
					{
						//Force a miss.
						DeactivateNote();
						gameObject.SetActive(false);
					}
				}
			}

			if (_autoPerfectRelease && Application.isPlaying && _holding && deltaTEnd > 0)
			{
				//Trigger a release input within code.
				OnTriggerInput(new InputEventData(RhythmClipData.TrackID, 1));
				DeactivateNote();
			}

			_endNote.position = GetNotePosition(deltaTEnd);
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