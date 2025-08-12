using System;
using System.Collections.Generic;
using Base.Presenter;
using Settings;
using UnityEngine;

namespace UI.Prefabs
{
	public class PrefabProvider : IPrefabProvider
	{
		private readonly IPrefabSettings _prefabSettings;
		private readonly IDictionary<Type, GameObject> _prefabs;

		public PrefabProvider(IPrefabSettings prefabSettings)
		{
			_prefabSettings = prefabSettings;
			_prefabs = new Dictionary<Type, GameObject>();

			CollectAllPrefabs();
		}

		private void CollectAllPrefabs()
		{
			foreach (var screenMode in _prefabSettings.ScreenModes)
				_prefabs.Add(screenMode.GetType(), screenMode.gameObject);

			foreach (var desktop in _prefabSettings.Desktops)
				_prefabs.Add(desktop.GetType(), desktop.gameObject);

			foreach (var presentation in _prefabSettings.Presentations)
				_prefabs.Add(presentation.GetType(), presentation.gameObject);

			foreach (var status in _prefabSettings.Statuses)
				_prefabs.Add(status.GetType(), status.gameObject);

			foreach (var floatingWindow in _prefabSettings.FloatingWindows)
				_prefabs.Add(floatingWindow.GetType(), floatingWindow.gameObject);

			foreach (var windowAdapter in _prefabSettings.WindowAdapters)
				_prefabs.Add(windowAdapter.GetType(), windowAdapter.gameObject);

			foreach (var window in _prefabSettings.Windows)
				_prefabs.Add(window.GetType(), window.gameObject);

			foreach (var widget in _prefabSettings.Widgets)
				_prefabs.Add(widget.GetType(), widget.gameObject);

			foreach (var shadedElement in _prefabSettings.ShadedElements)
				_prefabs.Add(shadedElement.GetType(), shadedElement.gameObject);
		}

		public GameObject GetPrefabOf(Type presenterType) =>
			_prefabs.TryGetValue(presenterType, out var prefab) ? prefab : null;

		public GameObject GetPrefabOf<TPresenter>() where TPresenter : PresenterBase, IPresenter =>
			GetPrefabOf(typeof(TPresenter));
	}
}