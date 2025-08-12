using System;

namespace Core.Workers.Material.Factory
{
	public interface IMaterialWorkerFactory
	{
		 bool TryGetWorker<TMaterialWorkerBase>(Type materialType, out TMaterialWorkerBase worker);
	}
}