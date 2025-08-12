using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Input {
	[System.Serializable]
	public class EnemyData : InputPackage {

		public EnemyInfo enemy_info;
		public ItemData[] enemy_items;
		// enemy_items
		public string xhr_ver;
	}
}