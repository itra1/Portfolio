using Base;
using Base.Controller;

namespace Materials.Controller
{
	public interface IMaterialController : IPreloadingAsync, IVisual, IUnloading
	{
		
	}
}