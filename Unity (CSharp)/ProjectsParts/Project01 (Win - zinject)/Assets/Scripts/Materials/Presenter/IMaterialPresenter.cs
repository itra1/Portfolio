using Base;
using Base.Presenter;
using Core.Materials.Data;

namespace Materials.Presenter
{
	public interface IMaterialPresenter : IPresenter, IPreloadableAsync
	{
		void SetParentArea(IParentArea parentArea);
		bool SetMaterial(MaterialData material);
	}
}