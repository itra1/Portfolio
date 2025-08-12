using Core.Installers.Base;
using Core.Network.Http;
using Core.Network.Socket;
using Core.Network.Socket.ActionTypes;
using Core.Network.Socket.IgnoredPackets;
using Core.Network.Socket.Packets.Outgoing.States.Common;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base;
using OutgoingState = Core.Network.Socket.Packets.Outgoing.States.Common.OutgoingState;

namespace Core.Installers
{
	public class NetworkCoreInstaller : AutoResolvingMonoInstaller<NetworkCoreInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<SocketActionTypesInfo>().AsSingle().NonLazy();
			BindInterfacesTo<IgnoredIncomingPackets>().AsSingle().NonLazy();
			BindInterfacesTo<SocketOptions>().AsSingle().NonLazy();
			BindInterfacesTo<SocketConnection>().AsSingle().NonLazy();
			
			BindInterfacesTo<HttpBaseUrl>().AsSingle().NonLazy();
			BindInterfacesTo<UnityWebRequestProvider>().AsSingle().NonLazy();
			BindInterfacesTo<UnityWebRequestAsyncProvider>().AsSingle().NonLazy();
			BindInterfacesTo<Authorization>().AsSingle().NonLazy();
			
			Bind<IOutgoingState>().To<OutgoingState>().AsSingle().NonLazy();
			BindInterfacesTo<OutgoingStateController>().AsSingle().NonLazy();
			Bind<IOutgoingTimersTickState>().To<OutgoingTimersTickState>().AsSingle().NonLazy();
			BindInterfacesTo<OutgoingTimersTickStateController>().AsSingle().NonLazy();
			
			base.InstallBindings();
		}

		protected override void ResolveAll()
		{
			Resolve<ISocketActionPackets>();
			Resolve<IIgnoredIncomingPackets>();
			Resolve<ISocketOptions>();
			Resolve<ISocketConnection>();
			Resolve<ISocketState>();
			Resolve<ISocketSender>();
			
			Resolve<IHttpBaseUrl>();
			Resolve<IHttpRequest>();
			Resolve<IHttpRequestAsync>();
			Resolve<IAuthorization>();
			
			Resolve<IOutgoingState>();
			Resolve<IOutgoingStateController>();
			Resolve<IOutgoingTimersTickState>();
			Resolve<IOutgoingTimersTickStateController>();
			
			base.ResolveAll();
		}
	}
}