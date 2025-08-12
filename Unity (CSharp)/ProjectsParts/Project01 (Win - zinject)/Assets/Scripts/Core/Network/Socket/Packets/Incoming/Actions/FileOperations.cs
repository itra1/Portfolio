using System;
using Core.Elements.Widgets.Base.Data;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Data;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("file_create")]
	[SocketAction("file_delete")]
	[SocketAction("file_update")]
	public class FileOperations : MaterialOperations<MaterialData>
	{
		protected override MaterialData GetMaterial()
		{
			var material = default(MaterialData);
			
			if (IsRemoving)
				material = Materials.Get(Content.GetULong(IncomingPacketDataKey.Id));
			
			if (material != null) 
				return material;
			
			var concreteType = GetConcreteTypeBaseOn<WindowMaterialData>() ?? GetConcreteTypeBaseOn<WidgetMaterialData>();
			
			if (concreteType == null) 
				return null;
			
			material = MaterialParser.ParseOne(concreteType, Content);
			WorkerCoordinator.PerformActionAfterAddingToStorageOf(material);
			
			return material;
		}

		private Type GetConcreteTypeBaseOn<TMaterialData>() where TMaterialData : MaterialData => 
			WorkerCoordinator.DefineConcreteTypeFrom(typeof(TMaterialData), Content);
	}
}