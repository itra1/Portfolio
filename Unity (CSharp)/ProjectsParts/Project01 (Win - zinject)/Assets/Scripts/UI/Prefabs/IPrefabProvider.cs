using System;
using Base.Presenter;
using UnityEngine;

namespace UI.Prefabs
{
	public interface IPrefabProvider
	{
		GameObject GetPrefabOf(Type presenterType);
		GameObject GetPrefabOf<TPresenter>() where TPresenter : PresenterBase, IPresenter;
	}
}