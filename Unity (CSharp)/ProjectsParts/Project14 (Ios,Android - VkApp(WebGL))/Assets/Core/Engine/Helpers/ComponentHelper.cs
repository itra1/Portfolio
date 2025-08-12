using UnityEngine;

public static class ComponentHelper
{

	public static T GetOrAddComponent<T>(this Component mb)
	where T : Component
	{
		return mb.gameObject.GetOrAddComponent<T>();
	}
	public static T GetOrAddComponent<T>(this GameObject mb)
	where T : Component
	{
		if (mb.TryGetComponent<T>(out var component))
		{
			return (T)component;
		}
		else
		{
			return mb.AddComponent<T>();
		}
	}
	public static bool RemaneGameObjectByType(this Component component)
	{
		if (component.gameObject.name != component.GetType().Name)
		{
			component.gameObject.name = component.GetType().Name;
			return true;
		}
		return false;
	}

}