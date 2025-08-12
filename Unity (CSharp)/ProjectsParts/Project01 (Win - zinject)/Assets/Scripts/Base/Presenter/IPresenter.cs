using UnityEngine;

namespace Base.Presenter
{
	public interface IPresenter : IInitializedChild<RectTransform>, IRectTransformable, IRectContainer, IAlignable, IVisual, IVisible, IUnloadable
	{
	}
}