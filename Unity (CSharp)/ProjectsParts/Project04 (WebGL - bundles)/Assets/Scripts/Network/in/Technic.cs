using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Input {
	public class Technic : InputPackage {

		public string playerName;

		public string battle_id;
		public string technic_id;
		public string x;
		public string y;
		public int posX {
			get { return Int32.Parse(x??"0"); }
		}
		public int posY {
			get { return Int32.Parse(y??"0"); }
		}
		public string army;
		public string killed;
		public string rem_speed;
		public string complete;
		public string active;
		public string attack_x;
		public string attack_y;
		public int? owner_pid;
		public string tbid;
		public string durability_cur;
		public string name;
		public string level;
		public string speed;
		public string race;
		public string distance_min;
		public string distance_max;
		public string radius;
		public string damage_min;
		public string damage_max;
		public string durability;
		public string price;
		public string price_tl;
	}
}