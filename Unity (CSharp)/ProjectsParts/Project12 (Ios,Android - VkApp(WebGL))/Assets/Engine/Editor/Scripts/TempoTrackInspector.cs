using System.Collections.Generic;
using Engine.Scripts.Timelines;
using Engine.Scripts.Timelines.Playables.Tempo;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.UIElements;

namespace Game.Editor
{
	[CustomEditor(typeof(TempoTrack), true)]
	public class TempoTrackInspector : UIElementsInspector
	{
		protected override List<string> propertiesToExclude => new() { _itemPropertyName };

		protected const string _itemPropertyName = "m_StartItem";

		protected TempoTrack _tempoTrack;

		protected Button _setTempoButton;
		protected Button _clearMarkerButton;
		protected FloatField _bpmField;
		protected FloatField _offsetField;
		protected FloatField _durationField;
		protected Toggle _withOnBeatMarker;
		protected Toggle _withOffBeatMarker;
		protected Toggle _quarterBeatMarker;
		protected Toggle _octoBeatMarker;
		protected VisualElement _tempoContainer;

		public override VisualElement CreateInspectorGUI()
		{
			var container = new VisualElement();

			_tempoTrack = target as TempoTrack;

			var UIElementFields = CreateUIElementInspectorGUI(serializedObject, propertiesToExclude);

			_tempoContainer = new VisualElement();

			_bpmField = new FloatField("BPM")
			{
				value = (_tempoTrack.timelineAsset as RhythmTimelineAsset)?.Bpm ?? 120
			};

			_offsetField = new FloatField("Offset")
			{
				value = 0
			};
			_durationField = new FloatField("Duration")
			{
				value = (float) _tempoTrack.timelineAsset.duration
			};

			_withOnBeatMarker = new Toggle("With On Beat Marker")
			{
				value = true
			};
			_withOffBeatMarker = new Toggle("With Off Beat Marker")
			{
				value = true
			};
			_quarterBeatMarker = new Toggle("Quarter Beat Marker")
			{
				value = true
			};
			_octoBeatMarker = new Toggle("Octo Beat Marker")
			{
				value = true
			};

			_setTempoButton = new Button
			{
				text = "SetTempo"
			};
			_setTempoButton.clickable.clicked += SetTempo;

			_clearMarkerButton = new Button
			{
				text = "Clear Markers"
			};
			_clearMarkerButton.clickable.clicked += ClearMarkers;

			_tempoContainer.Add(_bpmField);
			_tempoContainer.Add(_offsetField);
			_tempoContainer.Add(_durationField);
			_tempoContainer.Add(_withOnBeatMarker);
			_tempoContainer.Add(_withOffBeatMarker);
			_tempoContainer.Add(_quarterBeatMarker);
			_tempoContainer.Add(_octoBeatMarker);
			_tempoContainer.Add(_setTempoButton);
			_tempoContainer.Add(_clearMarkerButton);

			var addTempoFoldout = new Foldout
			{
				text = "Add Tempo"
			};
			addTempoFoldout.Add(_tempoContainer);

			container.Add(UIElementFields);
			container.Add(addTempoFoldout);

			return container;
		}

		private void SetTempo()
		{
			var step = 60d / _bpmField.value;
			var duration = _durationField.value;
			var stepCount = duration / step;
			var quadroCount = duration / (step / 4);
			var octoCount = duration / (step / 8);

			if (_withOnBeatMarker.value)
			{
				for (double i = 0; i < stepCount; i++)
				{
					var tempoMarker = _tempoTrack.CreateMarker<TempoMarker>((step * i) + _offsetField.value);
					tempoMarker.name = "BPM On Beat Marker";
					tempoMarker.ID = _tempoTrack.OnBeat?.ID ?? 0;
				}
			}

			var halfStep = step / 2f;

			if (_withOffBeatMarker.value)
			{
				for (double i = 0; i < stepCount; i++)
				{
					var tempoMarker = _tempoTrack.CreateMarker<TempoMarker>((step * i) - halfStep + _offsetField.value);
					tempoMarker.name = "BPM Off Beat Marker";
					tempoMarker.ID = _tempoTrack.OffBeat?.ID ?? 1;
				}
			}

			if (_quarterBeatMarker.value)
			{
				var quarterStep = step / 4f;
				for (double i = 1; i < quadroCount; i += 2)
				{
					var tempoMarker = _tempoTrack.CreateMarker<TempoMarker>((quarterStep * i) + _offsetField.value);
					tempoMarker.name = "Quarter Beat Marker";
					tempoMarker.ID = _tempoTrack.OffBeat?.ID ?? 2;
				}
			}

			if (_octoBeatMarker.value)
			{
				var octoStep = step / 8f;
				for (double i = 1; i < octoCount; i += 2)
				{
					var tempoMarker = _tempoTrack.CreateMarker<TempoMarker>((octoStep * i) + _offsetField.value);
					tempoMarker.name = "Octo Beat Marker";
					tempoMarker.ID = _tempoTrack.OffBeat?.ID ?? 3;
				}
			}

			TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved | RefreshReason.WindowNeedsRedraw);
		}

		private void ClearMarkers()
		{
			var markers = _tempoTrack.GetMarkers();
			foreach (var marker in markers)
			{
				_ = _tempoTrack.DeleteMarker(marker);
			}
		}
	}
}