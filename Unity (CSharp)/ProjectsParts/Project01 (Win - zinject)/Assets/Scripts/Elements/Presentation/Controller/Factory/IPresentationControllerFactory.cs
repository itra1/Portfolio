using Core.Elements.Presentation.Data;

namespace Elements.Presentation.Controller.Factory
{
	public interface IPresentationControllerFactory
	{
		IPresentationController Create(PresentationMaterialData material, PresentationAreaMaterialData areaMaterial);
	}
}