using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditRun {

  public class EditRunLibrary : ScriptableObject {
		public List<GroupSpawnParametr> groupList;
		public List<SpawnObjectInfo> objectList;
		public Sprite backIcon;
	}

	[System.Serializable]
	public struct GroupSpawnParametr {
		public SpawnType type;
#if UNITY_EDITOR
		public Sprite sprite;
#endif
		public string title;
	}


}