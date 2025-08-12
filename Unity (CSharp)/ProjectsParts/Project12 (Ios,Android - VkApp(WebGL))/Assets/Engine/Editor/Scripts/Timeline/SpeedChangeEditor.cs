using Engine.Scripts.Timelines;
using Engine.Scripts.Timelines.Playables.NotesSpeed;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace Game.Editor.Scripts.Timeline
{
	[CustomTimelineEditor(typeof(NoteSpeedClip))]
	public class SpeedChangeEditor : ClipEditor
	{
		GUIStyle _textStyle;
		GUIStyle _pointStyle;
		public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
		{
			base.OnCreate(clip, track, clonedFrom);

			clip.displayName = " ";
		}

		public override void OnClipChanged(TimelineClip clip)
		{
			base.OnClipChanged(clip);

			if (clip.GetParentTrack().timelineAsset is not RhythmTimelineAsset timelineAsset)
				return;

			timelineAsset.CalcTimeline();

			EditorUtility.SetDirty(timelineAsset);
		}

		public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
		{

			base.DrawBackground(clip, region);

			_textStyle = new()
			{
				alignment = TextAnchor.MiddleCenter,
				normal = new GUIStyleState() { textColor = Color.white }
			};

			_pointStyle = new()
			{
				alignment = TextAnchor.MiddleCenter,
				normal = new GUIStyleState() { textColor = Color.yellow }
			};

			var parentAsset = clip.GetParentTrack().parent as RhythmTimelineAsset;

			var asset = clip.asset as NoteSpeedClip;
			asset.SetTimeIn(clip.start);
			asset.SetTimeOut(clip.end);
			asset.SetParentAsset(parentAsset);

			var start = parentAsset.SpawnRange.Max * (region.position.width / clip.duration);

			_ = EditorGUI.TextArea(new Rect((float) start, region.position.y, 1, region.position.height), $"*", _pointStyle);

			_ = EditorGUI.TextArea(new Rect(region.position.position.x, region.position.y, region.position.width, region.position.height), $"{asset.Speed * 100}% ({parentAsset.NoteSpeed * asset.Speed})", _textStyle);
		}
	}
}
