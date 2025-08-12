using Engine.Scripts.Inputs;
using Engine.Scripts.Timelines.Notes.Base;
using StringDrop;
using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.Scripts.Timelines.Notes
{
	/// <summary>
	/// A swipe Note is similar to a tap note except its input must have a direction.
	/// </summary>
	public class SwipeNote : TapNote
	{
		[FormerlySerializedAs("_swipeDirection")]
		[SerializeField] private Vector2 _swipeDirection;
		[SerializeField] private float _angleTolerance = 30;
		[SerializeField] private bool _failOnWrongDirectionSwipe = false;
		[StringDropList(typeof(NoteType))][SerializeField] private string _noteType;

		public override string Type => _noteType;


		/// <summary>
		/// Trigger an input on the note. Detect swipes.
		/// </summary>
		/// <param name="inputEventData">The input event data.</param>
		public override void OnTriggerInput(InputEventData inputEventData)
		{
			if (!inputEventData.Swipe && !inputEventData.TouchAsSwipe)
				return;

			var swipeAngleOffset = Vector2.Angle(_swipeDirection, inputEventData.Direction);

			if (swipeAngleOffset > _angleTolerance && !inputEventData.TouchAsSwipe)
			{

				if (!_failOnWrongDirectionSwipe)
					return;

				_isTriggered = true;

				InvokeNoteTriggerEvent(inputEventData, _rhythmClipData.RealDuration, 100);
				RhythmClipData.TrackObject.RemoveActiveNote(this);
				DeactivateNote();
				return;
			}

			_isTriggered = true;

			var perfectTime = _rhythmClipData.RealDuration / 2f;
			var timeDifference = TimeFromActivate - perfectTime;
			var timeDifferencePercentage = Mathf.Abs((float) (100f * timeDifference)) / perfectTime;

			InvokeNoteTriggerEvent(inputEventData, timeDifference, (float) timeDifferencePercentage);
			RhythmClipData.TrackObject.RemoveActiveNote(this);
			DeactivateNote();
		}
	}
}