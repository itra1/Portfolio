using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : Singleton<MapManager> {

	public MapBehaviour map;
	public SpriteRenderer fog;

	public int mapNum;

	private void Start() { }

	public MapBehaviour SetMap(MapBehaviour mb) {

		if (map != null)
			DestroyImmediate(map.gameObject);

		GameObject inst = Instantiate(mb.gameObject, Vector3.zero, Quaternion.identity);
		inst.transform.SetParent(GetComponent<GameManager>().world);
		map = inst.GetComponent<MapBehaviour>();
		map.DrawGride();
		return map;
	}

	public void SetMapFog(Vector3 posit) {
		fog.material.SetVector("_Point",
				new Vector4(posit.x, posit.y, posit.z, (map.playerSize == 5 ? 3000 : 850)));
	}

	public void SceneLoader(int sceneNum, Action OnComplited) {

		if (sceneNum == 3)
			sceneNum = 117;
    
		BundleManager.GetBundle("maps/scenes.mb" + sceneNum, "MapBehaviour", (obj) => {
			Debug.Log(obj);
			MapBehaviour mapBeh = MapManager.Instance.SetMap((obj as GameObject).GetComponent<MapBehaviour>());
			MiniMapBehaviour.Instance.SetMap(mapBeh.miniMap);

#if UNITY_EDITOR

			Debug.Log("LoadScene");
			SceneManager.LoadScene("map" + sceneNum, LoadSceneMode.Additive);
			if (OnComplited != null) OnComplited();

#else
			
			BundleManager.GetBundle("maps/scenes.map" + sceneNum, "map" + sceneNum, (scenes) => {
				SceneManager.LoadScene((scenes as string[])[0], LoadSceneMode.Additive);
				
				//SetMap((map as GameObject).GetComponent<MapBehaviour>());
				
				//MiniMapBehaviour.Instance.SetMap(mapBeh.GetComponent<MapBehaviour>().miniMap);
				if (OnComplited != null) OnComplited();
			}, true);
#endif

		});
	}

	public void LoadMap(int mapNum, Action OnComplited) {

		this.mapNum = mapNum;

		BundleManager.GetBundle("maps/maps.map" + mapNum, "map" + mapNum, (map) => {

			SetMap((map as GameObject).GetComponent<MapBehaviour>());

			MiniMapBehaviour.Instance.SetMap(this.map.miniMap);

			if (OnComplited != null) OnComplited();
		});

	}

#if UNITY_EDITOR

	private void OnDrawGizmos() {
		CenterGizmoDraw();
		Gizmos.color = Color.green;
	}

	private void CenterGizmoDraw() {
		if (map != null)
			Gizmos.DrawIcon(map.startGridePoint, "Marcker.png", false);
	}

#endif

}
