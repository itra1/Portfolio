using Core.Configs;
using Core.Materials.Parsing;
using Core.Materials.Storage;
using Core.Network.Http;
using Core.Network.Socket.ActionTypes;
using Core.Options;
using Core.Settings;
using Core.Workers.Material.Coordinator;
using Core.Workers.Material.Factory;
using Editor.Build.Checksum;
using Editor.Build.Messengers.Telegram;
using Editor.Materials.Parsing;
using Settings;
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
			Container.BindInterfacesTo<ConfigProvider>().AsSingle().NonLazy();
			
			Container.BindInterfacesTo<ProjectSettings>()
				.FromScriptableObject(Resources.Load<ProjectSettings>("ProjectSettings"))
				.AsSingle().NonLazy(); 
			Container.BindInterfacesTo<UISettings>()
				.FromScriptableObject(Resources.Load<UISettings>("UISettings"))
				.AsSingle().NonLazy();
			Container.BindInterfacesTo<PrefabSettings>()
				.FromScriptableObject(Resources.Load<PrefabSettings>("PrefabSettings"))
				.AsSingle().NonLazy();
			
			Container.BindInterfacesTo<HttpBaseUrl>().AsSingle().NonLazy();
			Container.BindInterfacesTo<UnityWebRequestProvider>().AsSingle().NonLazy();
			Container.BindInterfacesTo<ChecksumCalculation>().AsSingle().NonLazy();
			Container.BindInterfacesTo<TelegramPostingProvider>().AsSingle().NonLazy();
			Container.BindInterfacesTo<MaterialWorkerFactory>().AsSingle().NonLazy();
			Container.BindInterfacesTo<MaterialWorkerCoordinator>().AsSingle().NonLazy();
			Container.BindInterfacesTo<MemberInfoHelper>().AsSingle().NonLazy();
			Container.BindInterfacesTo<MaterialDataStorage>().AsSingle().NonLazy();
			Container.BindInterfacesTo<MaterialDataParsingHelper>().AsSingle().NonLazy();
			Container.BindInterfacesTo<MaterialDataSerializeHelper>().AsSingle().NonLazy();
			Container.BindInterfacesTo<ApplicationOptions>().AsSingle().NonLazy();
			Container.BindInterfacesTo<SocketActionTypesInfo>().AsSingle().NonLazy();
			
			ResolveAll();
		}

		private void ResolveAll()
		{
			Container.Resolve<IConfig>();
			Container.Resolve<IProjectSettings>();
			Container.Resolve<IUISettings>();
			Container.Resolve<IPrefabSettings>();
			Container.Resolve<IHttpBaseUrl>();
			Container.Resolve<IHttpRequest>();
			Container.Resolve<IChecksum>();
			Container.Resolve<ITelegramPostingProvider>();
			Container.Resolve<IMaterialWorkerFactory>();
			Container.Resolve<IMaterialWorkerCoordinator>();
			Container.Resolve<IMemberInfoHelper>();
			Container.Resolve<IMaterialDataStorage>();
			Container.Resolve<IMaterialDataParsingHelper>();
			Container.Resolve<IMaterialDataSerializeHelper>();
			Container.Resolve<IApplicationOptions>();
			Container.Resolve<IApplicationOptionsSetter>();
			Container.Resolve<ISocketActionPackets>();
		}
	}
}