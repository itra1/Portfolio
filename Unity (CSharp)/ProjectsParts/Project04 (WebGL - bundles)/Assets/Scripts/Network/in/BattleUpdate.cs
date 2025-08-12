using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Network.Input {

	/// <summary>
	/// Старт уровня
	/// </summary>
	[System.Serializable]
	public class BattleUpdate : InputPackage {

		public string id_answer;
		public Data data;
		public MovePosition[] poss_move;
		public MovePosition[] attack_cell;
		public MovePosition[] magic_cell;
		public MovePosition[] map_effects;
		public MyPlayer my_player;
		public int enemy_id;
		public BattleInfo battle_info;
		public int? settings;
		public string xhr_ver;

		[System.Serializable]
		public class Data {
			public Magic[] magic;
			public object players;
			public Dictionary<string, PlayerInfo> playersDict {
				get {
					try {
						return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PlayerInfo>>(players.ToString());
					} catch {
						return new Dictionary<string, PlayerInfo>();
					}
				}
			}
			public Army[] army_positions;
			public Technic[] technics;
			//public object constructions;
			public object constructions;
			public Dictionary<string, Constructions> constructions_parse = new Dictionary<string, Constructions>();
		}

		public void Init() {
			try {
				
				//data.constructions_parse = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Constructions>>(Newtonsoft.Json.JsonConvert.SerializeObject(data.constructions));
				data.constructions_parse = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Constructions>>(data.constructions.ToString());

				//data.constructions_parse = (Dictionary<string, Constructions>)MiniJSON.Json.Deserialize(MiniJSON.Json.Serialize(data.constructions));


			} catch {

			}

		}

	}

	public class Constructions {
		public string name;
		public string construction_id;
		public string x;
		public string y;
		public string active;
	}

	public class Magic {
		public int x;
		public int y;
		public string magic_id;
	}

}