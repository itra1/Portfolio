using System.Collections.Generic;
using System.Linq;
using Engine.Scripts.Timelines.Playables.NotesSpeed;
using UnityEditor.ShortcutManagement;
using UnityEditor.Timeline.Actions;
using UnityEngine.Timeline;

namespace Game.Core.Playables.SpeedChange
{
	public class TimeLineCastomAction : TimelineAction
	{
		public override bool Execute(ActionContext context)
		{
			var track = context.tracks.First() as NoteSpeedTrack;
			var clip = track.CreateClip<NoteSpeedClip>();
			clip.displayName = " ";
			clip.start = context.director.time;
			clip.duration = 0.5f;
			return true;
		}

		public override ActionValidity Validate(ActionContext context)
		{
			if (context.tracks.First().GetType() == typeof(NoteSpeedTrack))
				return ActionValidity.Valid;
			else
				return ActionValidity.NotApplicable;
		}

		//[TimelineShortcut("SampleClipAction", KeyCode.W)]
		public static void HandleShortCut(ShortcutArguments args)
		{
			_ = Invoker.InvokeWithSelected<TimeLineCastomAction>();
		}
	}
	[MenuEntry("Simple Menu Action")]
	[ApplyDefaultUndo]
	public class SetNameToTypeAction : TrackAction
	{
		public override ActionValidity Validate(IEnumerable<TrackAsset> items)
		{
			if (items.First().GetType() == typeof(NoteSpeedTrack))
				return ActionValidity.Valid;
			else
				return ActionValidity.NotApplicable;
		}
		public override bool Execute(IEnumerable<TrackAsset> items)
		{
			AppLog.Log("exec key");
			var track = items.First() as NoteSpeedTrack;
			var clip = track.CreateClip<NoteSpeedClip>();
			clip.displayName = " ";
			clip.start = 1;
			clip.duration = 0.5f;
			AppLog.Log(clip);
			//TimelineClip clip = new TimelineClip();
			//clip.duration = 5f;

			//TrackAsset track = timeline.GetTrack<CustomTrack>();
			//if (track != null)
			//{
			//	track.AddClip(clip);
			//}
			return true;
		}

		//[TimelineShortcut("SampleClipAction", KeyCode.W)]
		//public static void HandleShortCut(ShortcutArguments args)
		//{
		//	_ = Invoker.InvokeWithSelectedTracks<SetNameToTypeAction>();
		//}
	}
}
