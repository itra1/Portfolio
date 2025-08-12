using System;
using System.Collections.Generic;
using Base.Presenter;
using UnityEngine;

namespace Settings
{
	/// <summary>
	/// Устаревшее название - "UILibrary"
	/// </summary>
	public interface IPrefabSettings
	{
		IEnumerable<PresenterBase> ScreenModes { get; }
		IEnumerable<PresenterBase> Desktops { get; }
		IEnumerable<PresenterBase> Presentations { get; }
		IEnumerable<PresenterBase> Statuses { get; }
		IEnumerable<PresenterBase> FloatingWindows { get; }
		IEnumerable<Component> WindowAdapters { get; }
		IEnumerable<PresenterBase> Windows { get; }
		IEnumerable<PresenterBase> Widgets { get; }
		IEnumerable<PresenterBase> ShadedElements { get; }
		IEnumerable<GameObject> RenderStreamingObjects { get; }
		
		bool TryGetComponent<TComponent>(out TComponent component) where TComponent : Component;
		bool TryGetComponent<TComponent>(out TComponent component, Func<TComponent, bool> predicate) where TComponent : Component;
		bool TryGetComponent(Type type, out Component component);
		bool TryGetComponent(Type type, out Component component, Func<Component, bool> predicate);

#if UNITY_EDITOR
		void Actualize(IUISettings settings);
#endif
	}
}