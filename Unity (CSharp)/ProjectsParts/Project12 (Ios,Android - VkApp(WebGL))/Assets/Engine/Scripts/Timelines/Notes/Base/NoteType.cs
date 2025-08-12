using StringDrop;

namespace Engine.Scripts.Timelines.Notes.Base
{
	public struct NoteType
	{
		[StringDropItem] public const string Tap = "Tap";
		[StringDropItem] public const string Hold = "Hold";
		[StringDropItem] public const string Counter = "Counter";
		[StringDropItem] public const string SwipeDown = "SwipeDown";
		[StringDropItem] public const string SwipeUp = "SwipeUp";
		[StringDropItem] public const string SwipeLeft = "SwipeLeft";
		[StringDropItem] public const string SwipeRight = "SwipeRight";
	}
}
