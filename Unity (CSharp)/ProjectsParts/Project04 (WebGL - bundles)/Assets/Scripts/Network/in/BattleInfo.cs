using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Input {

	[System.Serializable]
	public class BattleInfo : InputPackage {

		public string battle_id;
		public int combat_type;
		public int round;
		public int status;
		public int map_id;
		public int pid;
		public string battle_lock;
		public string round_start_time;
		public long round_time_now;

		public long round_time_start {
			get { return long.Parse(round_start_time); }
		}

		public float time;

	}
}