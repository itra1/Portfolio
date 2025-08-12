using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using Zenject;

namespace Game.Editor
{
	[CustomEditor(typeof(RhythmTimelineAsset), true)]
	public class RhythmTimelineAssetInspector : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var container = new VisualElement();
			container.Add(new IMGUIContainer(OnInspectorGUI));

			PrintOpenButton(container);

			return container;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			RhythmTimelineAsset timelineAssset = target as RhythmTimelineAsset;
			if (target == null)
				return;

			timelineAssset.CalcTimeline();
		}

		private void PrintOpenButton(VisualElement container)
		{
			var button = new Button();
			button.style.marginTop = 30;
			button.text = "Open In RhythmDirector";
			button.clicked += () =>
			{
				var director = StaticContext.Container.Resolve<IRhythmDirector>();
				var playabledirector = FindFirstObjectByType<PlayableDirector>();
				var currentAsset = target as RhythmTimelineAsset;

				if (director == null)
				{
					Debug.LogWarning("The Rhythm Director could not be found in the scene.");
					return;
				}
				StaticContext.Container.Inject(currentAsset);
				director.SetRhythmTimelineAsset(currentAsset);

				Selection.SetActiveObjectWithContext(playabledirector.gameObject, playabledirector.gameObject);
			};

			container.Add(button);
		}
	}
}