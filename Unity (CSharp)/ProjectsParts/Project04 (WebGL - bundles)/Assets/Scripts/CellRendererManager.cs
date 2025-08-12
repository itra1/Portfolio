using System.Collections;
using System.Collections.Generic;
using Cells;
using UnityEngine;

public class CellRendererManager : MonoBehaviour {

	public CellLineRenderer prefab;
	private List<CellLineRenderer> lineList;

	public void RendererGride(List<Cell> cellList, float sizeCell) {

		HideAll();
		
		for (int i = 0; i < cellList.Count; i++) {

			CellLineRenderer line = GetInstance();
			line.CellRenderer(cellList[i].position, sizeCell);
			line.gameObject.SetActive(true);

		}
	}

	private void HideAll() {

		lineList.RemoveAll(x => x == null);

		lineList.ForEach(x=>x.gameObject.SetActive(false));
	}


	CellLineRenderer GetInstance() {
		CellLineRenderer inst = lineList.Find(x => !x.gameObject.activeInHierarchy);

		if (inst == null) {
			GameObject go = Instantiate(prefab.gameObject);
			go.transform.SetParent(transform);
			go.transform.localPosition = Vector3.zero;
			inst = go.GetComponent<CellLineRenderer>();
			lineList.Add(inst);
		}
		return inst;

	}


}
