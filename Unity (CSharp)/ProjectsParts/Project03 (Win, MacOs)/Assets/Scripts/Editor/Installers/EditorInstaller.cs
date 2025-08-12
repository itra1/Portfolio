using Settings;
using Settings.Prefabs;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Editor.Installers
{
	[InitializeOnLoad]
	public class EditorInstaller : EditorStaticInstaller<EditorInstaller>
	{
		static EditorInstaller()
		{
			EditorApplication.QueuePlayerLoopUpdate();
			Install();
		}

		public override void InstallBindings()
		{

			Container.BindInterfacesTo<Settings.Settings>()
				.FromInstance(Resources.Load<Settings.Settings>("Settings"))
				.AsSingle()
				.NonLazy();

			Container.BindInterfacesTo<PrefabSettings>()
				.FromInstance(Resources.Load<PrefabSettings>("PrefabSettings"))
				.AsSingle()
				.NonLazy();

			ResolveAll();
		}

		private void ResolveAll()
		{
			Container.Resolve<ISettings>();
		}
	}
}