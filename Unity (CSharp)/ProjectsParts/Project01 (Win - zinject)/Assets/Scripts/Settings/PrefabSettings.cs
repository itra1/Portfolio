using System;
using System.Collections.Generic;
using System.Linq;
using Base.Presenter;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using UnityEditor;
#endif

namespace Settings
{
	/// <summary>
	/// Устаревшее название - "UILibrary"
	/// </summary>
	[CreateAssetMenu(fileName = "PrefabSettings", menuName = "Settings/PrefabSettings", order = 2)]
	public class PrefabSettings : ScriptableObject, IPrefabSettings
	{
		[SerializeField] private PresenterBase[] _screenModes;
		[SerializeField] private PresenterBase[] _desktops;
		[SerializeField] private PresenterBase[] _presentations;
		[SerializeField] private PresenterBase[] _statuses;
		[SerializeField] private PresenterBase[] _floatingWindows;
		[SerializeField] private Component[] _windowAdapters;
		[SerializeField] private PresenterBase[] _windows;
		[SerializeField] private PresenterBase[] _widgets;
		[SerializeField] private GameObject[] _components;
		[SerializeField] private PresenterBase[] _shadedElements;
		[SerializeField] private GameObject[] _renderStreamingObjects;
		
		public IEnumerable<PresenterBase> ScreenModes => _screenModes;
		public IEnumerable<PresenterBase> Desktops => _desktops;
		public IEnumerable<PresenterBase> Presentations => _presentations;
		public IEnumerable<PresenterBase> Statuses => _statuses;
		public IEnumerable<PresenterBase> FloatingWindows => _floatingWindows;
		public IEnumerable<Component> WindowAdapters => _windowAdapters;
		public IEnumerable<PresenterBase> Windows => _windows;
		public IEnumerable<PresenterBase> Widgets => _widgets;
		public IEnumerable<PresenterBase> ShadedElements => _shadedElements;
		public IEnumerable<GameObject> RenderStreamingObjects => _renderStreamingObjects;
		
		public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : Component
		{
			foreach (var gameObject in _components)
			{
				if (gameObject.TryGetComponent(out component))
					return true;
			}
			
			component = null;
			return false;
		}
		
		public bool TryGetComponent<TComponent>(out TComponent component, Func<TComponent, bool> predicate) where TComponent : Component
		{
			foreach (var gameObject in _components)
			{
				if (gameObject.TryGetComponent(out component) && (predicate == null || predicate.Invoke(component)))
					return true;
			}
			
			component = null;
			return false;
		}
		
		public bool TryGetComponent(Type type, out Component component)
		{
			foreach (var gameObject in _components)
			{
				if (gameObject.TryGetComponent(type, out component))
					return true;
			}
			
			component = null;
			return false;
		}
		
		public bool TryGetComponent(Type type, out Component component, Func<Component, bool> predicate)
		{
			foreach (var gameObject in _components)
			{
				if (gameObject.TryGetComponent(type, out component) && (predicate == null || predicate.Invoke(component)))
					return true;
			}
			
			component = null;
			return false;
		}
		
#if UNITY_EDITOR
		public void Actualize(IUISettings settings)
		{
			_shadedElements = LoadPresenters(settings.ShadedElementsFolder);
			_screenModes = LoadPresenters(settings.ScreenModesFolder);
			_desktops = LoadPresenters(settings.DesktopsFolder);
			_presentations = LoadPresenters(settings.PresentationsFolder);
			_statuses = LoadPresenters(settings.StatusesFolder);
			_floatingWindows = LoadPresenters(settings.FloatingWindowsFolder);
			_windowAdapters = LoadComponent<IWindowPresenterAdapter>(settings.WindowAdaptersFolder);
			_windows = LoadPresenters(settings.WindowsFolder);
			_widgets = LoadPresenters(settings.WidgetsFolder);
			_components = LoadGameObjects(settings.ComponentsFolder);
			_renderStreamingObjects = LoadGameObjects(settings.RenderStreamingFolder);
			
			EditorUtility.SetDirty(this);
		}
		
		private PresenterBase[] LoadPresenters(string path) => 
			LoadAssets<PresenterBase>(path);
		
		private Component[] LoadComponent<TComponent>(string path) => 
			LoadAssets<GameObject>(path)
				.Select(gameObject => gameObject.GetComponent<TComponent>() as Component)
				.ToArray();
		
		private GameObject[] LoadGameObjects(string path) => 
			LoadAssets<GameObject>(path);
		
		private T[] LoadAssets<T>(string path) where T : Object => 
			Resources.LoadAll<T>(path);
#endif
	}
}