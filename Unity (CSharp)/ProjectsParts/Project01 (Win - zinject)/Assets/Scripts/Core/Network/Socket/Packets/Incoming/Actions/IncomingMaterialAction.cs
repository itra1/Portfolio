using Core.Materials.Parsing;
using Core.Materials.Storage;
using Core.Network.Socket.Packets.Incoming.States;
using Zenject;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	public abstract class IncomingMaterialAction : IncomingAction
	{
		protected IMaterialDataStorage Materials { get; }
		protected IMaterialDataParser MaterialParser { get; }

		protected IncomingMaterialAction()
		{
			var container = ProjectContext.Instance.Container;
			
			Materials = container.Resolve<IMaterialDataStorage>();
			MaterialParser = container.Resolve<IMaterialDataParser>();
		}
	}
}