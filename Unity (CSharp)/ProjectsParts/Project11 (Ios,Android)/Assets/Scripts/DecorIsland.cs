using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class DecorIsland : MonoBehaviour {

	public float horiz;
	public float vertival;
	public GameIsland island;
	public IslandDecorsManager manager;
	
	public bool CheckPosition() {
		for (int i = 0; i < island.wordIslandList.Count; i++) {
			for (int n = 0; n < island.wordIslandList[i].listLetter.Count; n++) {
				
				if (
					Mathf.Abs(island.wordIslandList[i].listLetter[n].transform.position.x - transform.position.x) <
					((island.wordIslandList[i].sizeLetter / 2) + horiz)
					&&
					Mathf.Abs(island.wordIslandList[i].listLetter[n].transform.position.y - transform.position.y) <
					((island.wordIslandList[i].sizeLetter / 2) + vertival)
				) {
					return false;
				}
				
			}
		}

		//for (int i = 0; i < manager.decors[PlayerManager.Instance.company.actualLocationNum].decorsList.Count; i++) {

		//	if(!manager.decors[PlayerManager.Instance.company.actualLocationNum].decorsList[i].gameObject.activeInHierarchy) continue;

		//	if (
		//			Mathf.Abs(manager.decors[PlayerManager.Instance.company.actualLocationNum].decorsList[i].transform.position.x - transform.position.x) <
		//			(manager.decors[PlayerManager.Instance.company.actualLocationNum].decorsList[i].horiz + horiz)
		//			&&
		//			Mathf.Abs(manager.decors[PlayerManager.Instance.company.actualLocationNum].decorsList[i].transform.position.y - transform.position.y) <
		//			(manager.decors[PlayerManager.Instance.company.actualLocationNum].decorsList[i].vertival + vertival)
		//		) {
		//		return false;
		//	}

		//}

		Sort();
		return true;
	}

	private void Sort() {
		GetComponent<SortingGroup>().sortingOrder = -Mathf.RoundToInt(transform.position.y * 1000);
	}

	private void OnDrawGizmos() {
		Gizmos.DrawLine(new Vector3(transform.position.x- horiz, transform.position.y, transform.position.z), new Vector3(transform.position.x + horiz, transform.position.y, transform.position.z));
		Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y- vertival, transform.position.z), new Vector3(transform.position.x, transform.position.y+ vertival, transform.position.z));
	}

}
