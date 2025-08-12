
using System.Collections.Generic;
using UGui.Screens.Base;

using UnityEngine;

namespace Settings.Prefabs
{
	public interface IPrefabSettings
	{
		IEnumerable<MonoBehaviour> ScreenList { get; }

#if UNITY_EDITOR
		void Actualize(ISettings settings);
#endif

	}
}
