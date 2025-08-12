using Core.Elements.Desktop.Data;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("widget_create")]
	[SocketAction("widget_delete")]
	[SocketAction("widget_update")]
	public class WidgetOperations : MaterialOperations<WidgetMaterialData>
	{
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			if (!IsCreating && !IsUpdating)
				return true;
			
			var contentAreaMaterials = Materials.GetList<ContentAreaMaterialData>();
			
			foreach (var contentAreaMaterial in contentAreaMaterials)
			{
				var materialId = contentAreaMaterial.MaterialId;
				
				if (materialId == null || materialId != Material.Id) 
					continue;
				
				if (!contentAreaMaterial.IsContainer)
					Material = Materials.Get<WidgetMaterialData>(materialId.Value);
				
				var parentId = contentAreaMaterial.ParentId;
				
				if (parentId == null) 
					continue;
				
				var areaMaterial = Materials.Get<DesktopAreaMaterialData>(parentId.Value);
				
				if (areaMaterial != null)
					WorkerCoordinator.UpdateChildrenAt(areaMaterial, contentAreaMaterials);
			}
			
			return true;
		}
	}
}