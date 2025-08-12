using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : Singleton<PlayerStatManager> {

	public StatePlayer prefabStatePlay;
	private List<StatePlayer> instanceStatsPlayrs = new List<StatePlayer>();

	public StatePlayer GetStatPlayer() {
		return GetInstanceStatePlayer();
	}


	private StatePlayer GetInstanceStatePlayer() {

		StatePlayer stat = instanceStatsPlayrs.Find(x => !x.gameObject.activeInHierarchy);

		if (stat == null) {
			GameObject newInst = Instantiate(prefabStatePlay.gameObject);
			stat = newInst.GetComponent<StatePlayer>();
			newInst.transform.SetParent(transform);
			instanceStatsPlayrs.Add(stat);
		}

		return stat;

	}


}
