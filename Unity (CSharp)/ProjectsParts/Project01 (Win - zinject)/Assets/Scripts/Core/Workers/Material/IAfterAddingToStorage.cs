using Core.Materials.Data;

namespace Core.Workers.Material
{
	public interface IAfterAddingToStorage
	{
		void PerformActionAfterAddingToStorageOf(MaterialData material);
	}
}