using UnityEngine;

namespace Engine.Scripts.Inputs
{
	/// <summary>
	/// Input event data tracks the input type.
	/// </summary>
	public class InputEventData
	{
		public int InputID;
		public int TrackID;
		public Vector2 Direction { get; set; }

		public virtual bool Tap => InputID == 0;
		public virtual bool Release => InputID == 1;
		public virtual bool Swipe => InputID == 2;
		public virtual bool TouchAsSwipe { get; set; } = false;

		public InputEventData(int trackID, int inputID)
		{
			TrackID = trackID;
			InputID = inputID;
		}

	}
}