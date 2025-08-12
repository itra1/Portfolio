using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : Singleton<WorldManager> {

	public List<WorldAbstract> worldList;

	public WorldAbstract GetWorld(WorldType type) {
		return worldList.Find(x => x.type == type);
	}
	
}

public enum WorldType {
	none = 0,
	menu = 1,
	locations = 2,
	levels = 3,
	game = 4
}