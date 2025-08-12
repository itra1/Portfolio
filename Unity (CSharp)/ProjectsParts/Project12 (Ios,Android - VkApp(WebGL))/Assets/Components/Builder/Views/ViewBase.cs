using Builder.Common;
using UnityEngine.UIElements;

namespace Builder.Views
{
	public abstract class ViewBase
	{
		protected BuildSession _buildData;
		protected VisualTreeAsset _viewPrefab;
		protected VisualElement _view;

		public abstract string Type { get; }
		public VisualElement View => _view;

		protected ViewBase(BuildSession buildData)
		{
			_buildData = buildData;
			LoadPrefab();
			CreateUi();
		}

		protected abstract void LoadPrefab();

		protected virtual void CreateUi()
		{
			_view = _viewPrefab.Instantiate();
		}

		public void SetVisible(bool isVisible)
		{
			_view.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;

			if (isVisible)
				Show();
		}

		public virtual void Show() { }
	}
}
