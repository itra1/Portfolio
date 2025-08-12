using Engine.Scripts.Base;
using Engine.Scripts.Inputs;
using Engine.Scripts.Managers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.EventReceivers
{
	public class TrackInputEventReceiver : MonoBehaviour, IInjection
	{
		[Tooltip("The ID of the track to listen to.")]
		[SerializeField] protected int _trackID = -1;
		[Tooltip("Optionally the track object, instead of the track ID.")]
		[SerializeField] protected TrackObject m_TrackObject;
		[Tooltip("An input was pressed on that track.")]
		[SerializeField] protected UnityEvent m_InputPressed;
		[Tooltip("An input was released on that track.")]
		[SerializeField] protected UnityEvent m_InputReleased;

		protected IRhythmDirector _rhythmDirector;

		[Inject]
		private void Constructor(IRhythmDirector rhythmDirector)
		{
			_rhythmDirector = rhythmDirector;
			_rhythmDirector.RhythmProcessor.OnTriggerInputEvent.AddListener(HandleOnTriggerInputEvent);

			if (m_TrackObject == null)
				return;

			for (int i = 0; i < _rhythmDirector.TrackObjects.Length; i++)
			{
				if (_rhythmDirector.TrackObjects[i] == m_TrackObject)
				{
					_trackID = i;
					break;
				}
			}
		}

		private void HandleOnTriggerInputEvent(InputEventData inputEventData)
		{
			if (_trackID != -1 && _trackID != inputEventData.TrackID)
				return;

			if (inputEventData.Tap)
				m_InputPressed.Invoke();

			if (inputEventData.Release)
				m_InputReleased.Invoke();
		}
	}
}
