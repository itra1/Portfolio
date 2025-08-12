using System.Collections.Generic;
using UnityEngine;

namespace Game.Common.Settings {
	public interface IPrefabSettings {
		IEnumerable<MonoBehaviour> ScreenList { get; }
		IEnumerable<MonoBehaviour> PopupList { get; }
	}
}
