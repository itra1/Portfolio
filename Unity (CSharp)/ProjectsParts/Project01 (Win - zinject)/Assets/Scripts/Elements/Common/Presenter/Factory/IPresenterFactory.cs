using Base.Presenter;
using UnityEngine;

namespace Elements.Common.Presenter.Factory
{
	public interface IPresenterFactory
	{
		TPresenter Create<TPresenter>(RectTransform parent) where TPresenter : PresenterBase, IPresenter;
	}
}