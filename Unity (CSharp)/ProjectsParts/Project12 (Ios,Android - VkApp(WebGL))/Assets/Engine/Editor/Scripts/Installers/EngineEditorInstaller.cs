
#if UNITY_EDITOR
using Engine.Editor.Scripts.Controllers;
using Engine.Scripts.Installers;
using Engine.Scripts.Managers;
using Engine.Scripts.Settings;
using Engine.Scripts.Timelines.Notes.Factorys;
using Game.Scripts.Scenes;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Game.Editor.Installers
{
	public class EngineEditorInstaller : EditorStaticInstaller<EngineEditorInstaller>
	{
		static EngineEditorInstaller()
		{
			EditorApplication.QueuePlayerLoopUpdate();
			Install();
		}

		public override void InstallBindings()
		{

		}

		public void Initiate()
		{
			// Грузим конфиги
			_ = StaticContext.Container.BindInterfacesTo<ScoreSettings>()
				.FromInstance(Resources.Load<ScoreSettings>("Settings/ScoreSettings")).AsSingle().NonLazy();

			var rhythmSettingsResources = Resources.Load<RhythmSettings>("Settings/RhythmSettings");
			_ = StaticContext.Container.Bind<RhythmSettings>().FromInstance(rhythmSettingsResources).AsSingle().NonLazy();

			var notesSettingsResources = Resources.Load<NotesSettingsInstaller>("Settings/NotesSettingsInstaller");
			StaticContext.Container.Inject(notesSettingsResources);
			notesSettingsResources.InstallBindings();

			// Биндим остальное

			_ = StaticContext.Container.BindInterfacesTo<DspTime>().AsSingle().NonLazy();
			var accuracyControllerEditor = StaticContext.Container.BindInterfacesTo<AccuracyControllerEditor>().AsSingle().NonLazy();
			StaticContext.Container.Inject(accuracyControllerEditor);

			var factory = StaticContext.Container.BindInterfacesTo<NoteFactory>().AsSingle().NonLazy();
			StaticContext.Container.Inject(factory);

			GameScene scemeObjects = Behaviour.FindFirstObjectByType<GameScene>();
			_ = StaticContext.Container.BindInterfacesTo<GameScene>().FromInstance(scemeObjects).AsSingle().NonLazy();

			_ = StaticContext.Container.BindInterfacesTo<RhythmProcessor>().AsSingle().NonLazy();
			_ = StaticContext.Container.BindInterfacesTo<RhythmDirector>().AsSingle().NonLazy();
		}
	}
}

#endif