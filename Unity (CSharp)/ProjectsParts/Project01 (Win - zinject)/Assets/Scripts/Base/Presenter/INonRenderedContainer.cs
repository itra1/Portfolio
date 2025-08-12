using UnityEngine;

namespace Base.Presenter
{
	public interface INonRenderedContainer
	{
		RectTransform NonRenderedContent { get; }
	}
}