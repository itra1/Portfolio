using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Input {
	[System.Serializable]
	public class EnemyInfo : InputPackage {

		public string player_img;
		public string pid;
		public string login;
		public string clan_id;
		public string level;
		public string rating;
		public string trauma_head;
		public string trauma_body;
		public string trauma_right;
		public string trauma_left;
		public string trauma_foots;
		public string armor_head;
		public string armor_body;
		public string armor_right;
		public string armor_left;
		public string armor_foots;
		public float hp;
		public float hp_max;
		public float mp;
		public float mp_max;
		
	}
}
 