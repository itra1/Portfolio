using Base.Controller;
using Core.Elements.Presentation.Data;
using Core.Elements.PresentationEpisode.Data;
using Cysharp.Threading.Tasks;

namespace Elements.Presentation.Controller
{
	public interface IPresentationController : IController, IPreloadingAsync
	{
		PresentationMaterialData Material { get; }
		PresentationAreaMaterialData AreaMaterial { get; }
		PresentationEpisodeMaterialData ActiveEpisodeMaterial { get; }
		PresentationEpisodeAreaMaterialData ActiveEpisodeAreaMaterial { get; }

		UniTask<bool> SetChildActiveAsync(ulong materialId);
	}
}