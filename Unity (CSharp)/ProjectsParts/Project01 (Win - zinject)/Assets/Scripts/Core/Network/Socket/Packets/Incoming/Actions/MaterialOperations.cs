using Core.Materials.Data;
using Core.Network.Socket.Packets.Incoming.Actions.Consts;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Workers.Material.Coordinator;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	/// <summary>
	/// Устаревшее название - "MaterialCUD"
	/// Базовый класс сокет-пакета с операциями над материалом
	/// </summary>
	public abstract class MaterialOperations<TMaterialData> : IncomingMaterialAction where TMaterialData : MaterialData
	{
		protected IMaterialWorkerCoordinator WorkerCoordinator { get; }
		
		protected TMaterialData Material { get; set; }
		
		protected bool IsCreating { get; private set; }
		protected bool IsRemoving { get; private set; }
		protected bool IsUpdating { get; private set; }
		
		protected MaterialOperations() => 
			WorkerCoordinator = ProjectContext.Instance.Container.Resolve<IMaterialWorkerCoordinator>();
		
		public override bool Parse()
		{
			var postfix = Alias.Split('_')[^1];
			
			switch (postfix)
			{
				case MaterialOperationAliasPostfix.Create:
					IsCreating = true;
					break;
				case MaterialOperationAliasPostfix.Delete:
					IsRemoving = true;
					break;
				case MaterialOperationAliasPostfix.Update:
					IsUpdating = true;
					break;
			}
			
			Material = GetMaterial();
			
			if (Material == null)
			{
				Debug.LogError($"Material is missing when trying to parse incoming packet {GetType().Name} with alias \"{Alias}\"");
				return false;
			}
			
			if (IsCreating)
			{
				Material = Materials.Add(Material, 
					material => WorkerCoordinator.PerformActionAfterAddingToStorageOf(material));
			}
			else if (IsRemoving)
			{
				Materials.Remove(Material);
			}
			else
			{
				Material = Materials.UpdateOrAdd(Material, 
					material => WorkerCoordinator.PerformActionAfterAddingToStorageOf(material));
			}
			
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			if (Material == null)
			{
				Debug.LogError($"Material is missing when trying to process incoming packet {GetType().Name} with alias \"{Alias}\"");
				return false;
			}
			
			return true;
		}
		
		protected virtual TMaterialData GetMaterial()
		{
			var material = default(TMaterialData);
			
			if (IsRemoving)
				material = Materials.Get<TMaterialData>(Content.GetULong(IncomingPacketDataKey.Id));
			
			return material ?? MaterialParser.ParseOne<TMaterialData>(Content);
		}
	}
}