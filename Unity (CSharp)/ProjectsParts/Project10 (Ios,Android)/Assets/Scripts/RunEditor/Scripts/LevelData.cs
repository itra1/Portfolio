using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditRun {

	/// <summary>
	/// Локация
	/// </summary>
	[System.Serializable]
	public class LevelData : ScriptableObject {

		public string title;
    public string description;
		public GameMode gameMode;
		public GameLocation location;
		public MoveVector moveVector;
		public GameMechanic gameFormat;
		public HealthType healthType;
    public RegionType region;
    public string sceneStart;

		public List<BlockBase> runBlocks;
    private Transform mapParent;

    private float _levelDistantion;
		public float levelDistantion {
			get {
				_levelDistantion = 0;
        runBlocks.ForEach(x=> _levelDistantion += x.blockDistantion);
				return _levelDistantion;
			}
		}

    public void Init() {

      mapParent = GameObject.Find("World").transform;
      DestroyExistsObjects();

      float startDistance = 0;

      runBlocks.ForEach(elem => {

        //elem.Init(startDistance, false);
        startDistance += elem.blockDistantion+2.5f;
      });

    }

    public void DestroyExistsObjects() {

      while (mapParent.childCount > 0)
        DestroyImmediate(mapParent.GetChild(0).gameObject);
    }



  }

}
