using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Настройки минимапа
/// </summary>
[System.Serializable]
public class MiniMapConfig {
	
	public Sprite miniMap;
	public Vector2 regionPointStart;
	public Vector2 regionPointEnd;

	public Vector3 _regionPointStart {
		get { return new Vector3(regionPointStart.x,0, regionPointStart.y); }
	}
	public Vector3 _regionPointEnd {
		get { return new Vector3(regionPointEnd.x, 0, regionPointEnd.y); }
	}


#if UNITY_EDITOR
	public void DrawRegion() {

		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(_regionPointStart, new Vector3(_regionPointEnd.x, 0, _regionPointStart.z));
		Gizmos.DrawLine(new Vector3(_regionPointStart.x, 0, _regionPointEnd.z), _regionPointEnd);

		Gizmos.DrawLine(_regionPointStart, new Vector3(_regionPointStart.x, 0, _regionPointEnd.z));
		Gizmos.DrawLine(new Vector3(_regionPointEnd.x, 0, _regionPointStart.z), _regionPointEnd);
		
	}
#endif

}
