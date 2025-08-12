using Components.Pipes;
using Core.Installers.Base;
using Environment.Microsoft.Windows.Apps.Office;
using Environment.Microsoft.Windows.Apps.Office.Server;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Converting.Deserializers;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Converting.Serializers;
using Environment.Netsoft.WebView;
using Environment.Netsoft.WebView.Deserializers;
using Environment.Netsoft.WebView.Serializer;

namespace Installers
{
	public class EnvironmentInstaller : AutoResolvingMonoInstaller<ElementsInstaller>
	{
		private readonly string _officeApplicationPath = $@"{UnityEngine.Application.streamingAssetsPath}/OfficeApp/OfficeControl.exe";

		public override void InstallBindings()
		{

			_ = BindInterfacesTo<PipeServerFactory>().AsSingle().NonLazy();

			// MS Office
			_ = Bind<IPacketSerializer>().To<PacketJsonSerializer>().AsSingle().NonLazy();
			_ = Bind<IPacketDeserializer>().To<PacketJsonDeserializer>().AsSingle().NonLazy();

			_ = Bind<string>()
								.FromInstance(_officeApplicationPath)
								.When(ctx => ctx.ObjectType == typeof(MsOfficeController));

			_ = Bind(typeof(IMsOfficePipeServer), typeof(IMsOfficePipeServerState), typeof(IMsOfficeRequestAsync))
					.To<MsOfficePipeServer>().AsSingle().NonLazy();

			_ = BindInterfacesTo<MsOfficeController>().AsSingle().NonLazy();

			// Qt Browser
			_ = Bind<IActionSerializer>().To<ActionSerializer>().AsSingle().NonLazy();
			_ = Bind<IActionDeserializer>().To<ActionDeserializer>().AsSingle().NonLazy();

			_ = BindInterfacesTo<WebViewFactory>().AsSingle().NonLazy();

			base.InstallBindings();
		}

		protected override void ResolveAll()
		{
			_ = Resolve<IPacketSerializer>();
			_ = Resolve<IPacketDeserializer>();

			_ = Resolve<IMsOfficeRequestAsync>();
			_ = Resolve<IMsOfficePipeServer>();

			_ = Resolve<IMsOfficeController>();

			base.ResolveAll();
		}
	}
}