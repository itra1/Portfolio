using System;
using System.Collections.Generic;
using Core.Materials.Data;
using Core.Workers.Material.Factory;
using Leguar.TotalJSON;

namespace Core.Workers.Material.Coordinator
{
	public class MaterialWorkerCoordinator : IMaterialWorkerCoordinator
	{
		private readonly IMaterialWorkerFactory _workerFactory;

		public MaterialWorkerCoordinator(IMaterialWorkerFactory workerFactor) => 
			_workerFactory = workerFactor;

		public void PerformActionAfterAddingToStorageOf(MaterialData material)
		{
			if (_workerFactory.TryGetWorker(material.GetType(), out IAfterAddingToStorage worker))
				worker.PerformActionAfterAddingToStorageOf(material);
		}

		public void UpdateChildrenAt<TParentMaterialData, TChildMaterialData>(TParentMaterialData parent, IReadOnlyList<TChildMaterialData> children) 
			where TParentMaterialData : AreaMaterialData
			where TChildMaterialData : AreaMaterialData
		{
			if (_workerFactory.TryGetWorker(parent.GetType(), out IUpdatingChildren worker))
				worker.UpdateChildrenAt(parent, children);
		}
		
		public Type DefineConcreteTypeFrom(Type materialType, JSON json)
		{
			return _workerFactory.TryGetWorker(materialType, out IDefiningConcreteType worker) 
				? worker.DefineConcreteTypeFrom(json) 
				: null;
		}
	}
}