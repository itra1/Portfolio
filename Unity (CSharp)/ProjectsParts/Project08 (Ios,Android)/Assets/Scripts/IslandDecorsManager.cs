using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandDecorsManager : MonoBehaviour {
	
	public List<DecorGroup> decors;

	public GameIsland island;

	public void PositionsDecor() {

		DecorGroup useFroup = null;
		
		for (int i = 0; i < decors.Count; i++) {
			decors[i].parent.SetActive(i == PlayerManager.Instance.company.actualLocationNum);
			if (i == PlayerManager.Instance.company.actualLocationNum)
				useFroup = decors[i];
		}

		useFroup.decorsList.ForEach(x=>x.gameObject.SetActive(false));

		if (PlayerManager.Instance.company.isBonusLevel || Tutorial.Instance.isTutorial) return;

		for (int i = 0; i < useFroup.decorsList.Count; i++) {

			int num = 0;

			do {

				useFroup.decorsList[i].transform.localPosition = new Vector3(Random.Range(-island.horizontal/2 + useFroup.decorsList[i].horiz, island.horizontal / 2 - useFroup.decorsList[i].horiz), Random.Range(-island.verticel / 2 + useFroup.decorsList[i].vertival, island.verticel / 2 - useFroup.decorsList[i].vertival), 0);
				num++;

				if(num == 11) break;

			} while (!useFroup.decorsList[i].CheckPosition());

			if (num == 11) continue;

			useFroup.decorsList[i].gameObject.SetActive(true);

		}
		
	}

	[System.Serializable]
	public class DecorGroup {
		public int locNum;
		public GameObject parent;
		public List<DecorIsland> decorsList;
	}
	

}
