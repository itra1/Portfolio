using Engine.Scripts.Inputs;
using Engine.Scripts.Timelines.Notes.Base;

namespace Engine.Scripts.Timelines.Notes.Common
{
	/// <summary>
	/// The note trigger event data is used to hold information about the trigger event.
	/// </summary>
	public class NoteTriggerEventData
	{
		public Note Note;
		public InputEventData InputEventData;

		public string EventType { get; set; }

		public bool IsLoss => EventType == NoteEventType.Miss
											 || EventType == NoteEventType.Early;

		public double TriggerDspTime;
		public double DspTimeDifference;
		public float DspTimeDifferencePercentage;

		public void SetTriggerData(InputEventData eventData, double dspTimeDiff, float dspTimeDiffPerc, double dspAdaptiveTime)
		{
			InputEventData = eventData;
			TriggerDspTime = dspAdaptiveTime;
			DspTimeDifference = dspTimeDiff;
			DspTimeDifferencePercentage = dspTimeDiffPerc;
			EventType = NoteEventType.Normal;
		}

		public void SetLoss(double dspAdaptiveTime, string eventType)
		{
			InputEventData = null;
			TriggerDspTime = dspAdaptiveTime;
			DspTimeDifference = 0;
			DspTimeDifferencePercentage = 100;
			EventType = eventType;
		}
	}
}