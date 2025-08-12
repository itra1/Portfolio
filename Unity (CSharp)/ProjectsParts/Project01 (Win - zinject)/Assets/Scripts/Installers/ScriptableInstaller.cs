using Core.Settings;
using Environment.Netsoft.WebView.Settings;
using Settings;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
	[CreateAssetMenu(fileName = "ScriptableInstaller", menuName = "Settings/ScriptableInstaller", order = 1)]
	public class ScriptableInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private ProjectSettings _projectSettings;
		[SerializeField] private PrefabSettings _prefabSettings;
		[SerializeField] private UISettings _uiSettings;
		[SerializeField] private NetsofrWebViewSettings _nsWebViewSettings;

		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<NetsofrWebViewSettings>().FromInstance(_nsWebViewSettings).AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<UISettings>().FromInstance(_uiSettings).AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<ProjectSettings>().FromInstance(_projectSettings).AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<PrefabSettings>().FromInstance(_prefabSettings).AsSingle().NonLazy();

			Resolve();
		}

		private void Resolve()
		{
			//_ = Container.Resolve<IProjectSettings>();
			//_ = Container.Resolve<IUISettings>();
		}
	}
}
