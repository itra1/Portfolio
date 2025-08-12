using System.Linq;
using Engine.Scripts.Timelines.Notes.Common;
using Engine.Scripts.Timelines.Playables;
using Engine.Scripts.Timelines.Playables.Tempo;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Game.Editor.Scripts.Timeline.Actions
{
	public class CreateHoldNoteAction : TimelineAction
	{
		public override bool Execute(ActionContext context)
		{
			var track = context.tracks.First() as RhythmTrack;
			var clip = track.CreateClip<RhythmClip>();
			clip.displayName = " ";

			var asset = clip.asset as RhythmClip;
			var assetPath = AssetDatabase.FindAssets("HoldNote t:ScriptableObject")[0];

			asset.RhythmPlayableBehaviour.SetNoteDefinition(AssetDatabase.LoadAssetAtPath<NoteDefinition>(AssetDatabase.GUIDToAssetPath(assetPath)));
			clip.duration = 1f;

			var position = System.Math.Max(0, context.director.time - 0.25);

			var tracksList = context.timeline.GetRootTracks();

			foreach (var trackAsset in tracksList)
			{
				if (trackAsset is TempoTrack tempoTrack)
				{
					var minDistance = 0.125d;
					var startPosition = position;
					var tempoTrackClips = tempoTrack.GetMarkers();
					foreach (var tempoClip in tempoTrackClips)
					{
						var currentDistance = System.Math.Abs(tempoClip.time - startPosition);

						if (currentDistance < minDistance)
						{
							minDistance = currentDistance;
							position = tempoClip.time;
						}
					}
				}
			}

			clip.start = position;
			EditorUtility.SetDirty(track);
			context.director.RebuildGraph();
			context.director.Evaluate();
			return true;
		}

		public override ActionValidity Validate(ActionContext context)
		{
			if (context.tracks == null || context.tracks.Count() == 0)
				return ActionValidity.NotApplicable;
			if (context.tracks.First().GetType() == typeof(RhythmTrack))
				return ActionValidity.Valid;
			else
				return ActionValidity.NotApplicable;
		}

		[TimelineShortcut("Create Hold Note", KeyCode.E)]
		public static void HandleShortCut(ShortcutArguments args)
		{
			_ = Invoker.InvokeWithSelected<CreateHoldNoteAction>();
		}
	}
}
