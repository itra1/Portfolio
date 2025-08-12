using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

public class HeartsListUi : MonoBehaviour {

	public List<OneHeartUi> heartList;

	[ExEventHandler(typeof(GameEvents.OnChangeHeart))]
	private void CheartsChange(GameEvents.OnChangeHeart hearts) {
		for (int i = 0; i < heartList.Count; i++) heartList[i].isActive = i < hearts.heart;
	}

}
