using System;
using System.Collections.Generic;
using Engine.Scripts.Timelines;
using Engine.Scripts.Timelines.Notes.Common;
using Engine.Scripts.Timelines.Playables;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace Game.Editor
{
	[CustomEditor(typeof(RhythmTrack), true)]
	public class RhythmTrackInspector : UIElementsInspector
	{
		private const string AUTO_FILL_NOTE_DEFINITION_GUID_KEY = "AUTO_FILL_NOTE_DEFINITION_GUID_KEY";

		[Serializable]
		public enum AddOption
		{
			ClearPreviousNotes,
			AddOnTop,
			ReplaceOnOverlap,
			DontReplaceOnOverlap
		}

		protected override List<string> propertiesToExclude => new() { _itemPropertyName };

		protected const string _itemPropertyName = "m_StartItem";

		protected RhythmTrack _rhythmTrack;

		protected EnumField _addOptionOption;
		protected Button _spawnNotesButton;
		protected Button _clearSectionButton;
		protected Button _clearClipsButton;
		protected Button _refreshTimeline;
		protected ObjectField _autoFillNoteDefinitionField;
		protected FloatField _autofillBpmField;
		protected FloatField _startTimeField;
		protected FloatField _endTimeField;
		protected FloatField _autofillNoteSpacingField;
		protected FloatField _autofillNoteLengthField;
		protected VisualElement _rhythmTrackContainer;

		protected List<TimelineClip> _cachedClipsInRange;

		public override VisualElement CreateInspectorGUI()
		{
			_cachedClipsInRange = new List<TimelineClip>();

			var container = new VisualElement();

			_rhythmTrack = target as RhythmTrack;

			var UIElementFields = CreateUIElementInspectorGUI(serializedObject, propertiesToExclude);

			_rhythmTrackContainer = new VisualElement();

			_autoFillNoteDefinitionField = new ObjectField("Note Definition")
			{
				objectType = typeof(NoteDefinition),
				value = LoadNoteDefinition()
			};
			_ = _autoFillNoteDefinitionField.RegisterValueChangedCallback(evt =>
			{
				SaveNoteDefinition(evt.newValue as NoteDefinition);
			});

			_autofillBpmField = new FloatField("BPM")
			{
				value = (_rhythmTrack.timelineAsset as RhythmTimelineAsset)?.Bpm ?? 120
			};

			_startTimeField = new FloatField("Start Time")
			{
				value = 0
			};
			_endTimeField = new FloatField("End Time")
			{
				value = (float) _rhythmTrack.timelineAsset.duration
			};

			_autofillNoteSpacingField = new FloatField("Note Spacing")
			{
				tooltip = "Note Spacing in Beat",
				value = 1
			};

			_autofillNoteLengthField = new FloatField("Note Length")
			{
				tooltip = "Note Length in Beat",
				value = 1
			};

			_addOptionOption = new EnumField("Add Option", AddOption.ClearPreviousNotes);

			_spawnNotesButton = new Button
			{
				text = "Autofill Track Notes"
			};
			_spawnNotesButton.clickable.clicked += AutofillTrack;

			_clearSectionButton = new Button
			{
				text = "Clear Section"
			};
			_clearSectionButton.clickable.clicked += ClearSection;

			_clearClipsButton = new Button
			{
				text = "Clear All Notes"
			};
			_clearClipsButton.clickable.clicked += ClearAllTrack;

			_refreshTimeline = new Button
			{
				text = "Refresh Editor"
			};
			_refreshTimeline.clickable.clicked += RefreshTimeline;

			_rhythmTrackContainer.Add(_autoFillNoteDefinitionField);
			_rhythmTrackContainer.Add(_autofillBpmField);
			_rhythmTrackContainer.Add(_autofillNoteLengthField);
			_rhythmTrackContainer.Add(_autofillNoteSpacingField);
			_rhythmTrackContainer.Add(_startTimeField);
			_rhythmTrackContainer.Add(_endTimeField);
			_rhythmTrackContainer.Add(_addOptionOption);
			_rhythmTrackContainer.Add(_spawnNotesButton);
			_rhythmTrackContainer.Add(_clearSectionButton);
			_rhythmTrackContainer.Add(_clearClipsButton);
			_rhythmTrackContainer.Add(_refreshTimeline);
			var addRhythmFoldout = new Foldout
			{
				text = "Rhythm Track Autofill Tools"
			};
			addRhythmFoldout.Add(_rhythmTrackContainer);

			container.Add(UIElementFields);
			container.Add(addRhythmFoldout);

			return container;
		}

		private NoteDefinition LoadNoteDefinition()
		{
			var guid = EditorPrefs.GetString(AUTO_FILL_NOTE_DEFINITION_GUID_KEY, "");
			if (string.IsNullOrEmpty(guid))
				return null;

			var guidToAssetPath = AssetDatabase.GUIDToAssetPath(guid);
			if (string.IsNullOrEmpty(guidToAssetPath))
				return null;

			return AssetDatabase.LoadAssetAtPath<NoteDefinition>(guidToAssetPath);
		}

		private void SaveNoteDefinition(NoteDefinition noteDefinition)
		{
			if (noteDefinition == null)
			{
				EditorPrefs.SetString(AUTO_FILL_NOTE_DEFINITION_GUID_KEY, "");
				return;
			}

			var assetPath = AssetDatabase.GetAssetPath(noteDefinition);
			var guid = AssetDatabase.AssetPathToGUID(assetPath);
			EditorPrefs.SetString(AUTO_FILL_NOTE_DEFINITION_GUID_KEY, guid);
		}

		private void AutofillTrack()
		{
			var step = 60d / _autofillBpmField.value;
			var startTime = _startTimeField.value;
			var endTime = _endTimeField.value;
			float noteSpacing = _autofillNoteSpacingField.value;
			float noteLength = _autofillNoteLengthField.value;

			if (startTime < 0)
			{
				startTime = 0;
			}
			if (startTime > endTime)
			{
				Debug.LogError(_endTimeField.text + " cannot be smaller than the start time.");
				return;
			}
			if (noteSpacing < 0)
			{
				Debug.LogError(_autofillNoteSpacingField.text + " cannot be negative");
				return;
			}
			if (noteLength < 1)
			{
				Debug.LogError(_autofillNoteSpacingField.text + " cannot be 0 or negative");
				return;
			}

			switch ((AddOption) _addOptionOption.value)
			{
				case AddOption.AddOnTop:
					AddNotes(startTime, endTime, step, noteLength, noteSpacing);
					break;
				case AddOption.ClearPreviousNotes:
					ClearSection();
					AddNotes(startTime, endTime, step, noteLength, noteSpacing);
					break;
				case AddOption.ReplaceOnOverlap:
					AddNotesOverlap(startTime, endTime, step, noteLength, noteSpacing, true);
					break;
				case AddOption.DontReplaceOnOverlap:
					AddNotesOverlap(startTime, endTime, step, noteLength, noteSpacing, false);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			RefreshTimeline();
		}

		protected virtual void AddNotes(float startTime, float endTime, double step, float noteLength, float noteSpacing)
		{
			var stepCount = (endTime - startTime) / (step * (noteSpacing + noteLength));

			//fill notes by beat spacing
			for (double i = 0; i < stepCount; i++)
			{
				var clipStart = (step * i * (noteSpacing + noteLength)) + startTime;
				var clipDuration = noteLength * step;

				var clip = _rhythmTrack.CreateClip<RhythmClip>();
				var clipAsset = clip.asset as RhythmClip;
				clipAsset.RhythmPlayableBehaviour.SetNoteDefinition(_autoFillNoteDefinitionField.value as NoteDefinition);

				clip.start = clipStart;
				clip.duration = clipDuration;
			}
		}

		protected virtual void AddNotesOverlap(float startTime, float endTime, double step, float noteLength, float noteSpacing, bool replace)
		{
			var stepCount = (endTime - startTime) / (step * (noteSpacing + noteLength));

			//fill notes by beat spacing
			for (int i = 0; i < stepCount; i++)
			{
				var clipStart = (step * i * (noteSpacing + noteLength)) + startTime;
				var clipDuration = noteLength * step;

				var clips = GetClipsInRange(clipStart, clipStart + clipDuration);

				if (clips.Count != 0)
				{
					if (replace == false)
					{
						continue;
					}

					for (int j = 0; j < clips.Count; j++)
					{
						_ = _rhythmTrack.timelineAsset.DeleteClip(clips[j]);
					}
				}

				var clip = _rhythmTrack.CreateClip<RhythmClip>();
				var clipAsset = clip.asset as RhythmClip;
				clipAsset.RhythmPlayableBehaviour.SetNoteDefinition(_autoFillNoteDefinitionField.value as NoteDefinition);

				clip.start = clipStart;
				clip.duration = clipDuration;
			}
		}

		private List<TimelineClip> GetClipsInRange(double start, double end)
		{
			_cachedClipsInRange.Clear();

			var clips = _rhythmTrack.GetClips();

			foreach (var clip in clips)
			{
				if (clip.end < start || clip.start > end)
				{
					continue;
				}
				_cachedClipsInRange.Add(clip);
			}

			return _cachedClipsInRange;
		}

		private void RefreshTimeline()
		{
			TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved | RefreshReason.WindowNeedsRedraw);
		}

		private void ClearAllTrack()
		{
			var clips = _rhythmTrack.GetClips();

			foreach (var clip in clips)
			{
				_ = _rhythmTrack.timelineAsset.DeleteClip(clip);
			}

			RefreshTimeline();
		}

		private void ClearSection()
		{
			ClearSection(_startTimeField.value, _endTimeField.value);

		}

		private void ClearSection(float startTime, float endTime)
		{
			var clips = _rhythmTrack.GetClips();

			foreach (var clip in clips)
			{
				if (clip.end < startTime || clip.start > endTime)
				{
					return;
				}
				_ = _rhythmTrack.timelineAsset.DeleteClip(clip);
			}

			RefreshTimeline();
		}
	}
}