using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LettersController))]
public class LettersControllerEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Find")) {
			((LettersController)target).FindPositions();
		}

	}
}

#endif

public class LettersController : MonoBehaviour {
	public List<PositionParam> positionVariant = new List<PositionParam>();

	public List<AlphaFloatBehaviour> alphaListPrefabs = new List<AlphaFloatBehaviour>();
	public List<AlphaFloatBehaviour> alphaList = new List<AlphaFloatBehaviour>();

	public AlphaFloatBehaviour prefab;

	public void SetData() {

		HideExists();

		List<string> letters = PlayerManager.Instance.company.GetActualLevel().randomLetters;

		List<Vector3> positions = positionVariant.Find(x => x.positions.Count == letters.Count).positions;

		for (int i = 0; i < letters.Count; i++) {
			AlphaFloatBehaviour afb = GetInstance();
			afb.gameObject.SetActive(true);
			alphaList.Add(afb);
			afb.transform.localPosition = positions[i];
			afb.alpha = letters[i];
			afb.GetComponent<SortingGroup>().sortingOrder = (positions[i].y > 0 ? -1 : 0);
		}

	}

	void HideExists() {
		alphaListPrefabs.ForEach(x=>x.gameObject.SetActive(false));
		alphaList.Clear();
	}
	
	public AlphaFloatBehaviour GetInstance() {
		AlphaFloatBehaviour afb = alphaListPrefabs.Find(x => !x.gameObject.activeInHierarchy);
		if (afb == null) {
			GameObject inst = Instantiate(prefab.gameObject);
			inst.transform.SetParent(transform);
			afb = inst.GetComponent<AlphaFloatBehaviour>();
			alphaListPrefabs.Add(afb);
		}
		return afb;
	}

	public void FindPositions() {

		AlphaFloatBehaviour[] child = transform.GetComponentsInChildren<AlphaFloatBehaviour>();

		List<Vector3> useList = new List<Vector3>();

		for (int i = 0; i < child.Length; i++)
			useList.Add(child[i].transform.localPosition);

		PositionParam par = new PositionParam();
		par.positions = useList;

		positionVariant.Add(par);

	}

	public void ShowLetter() {
		StartCoroutine(ShowLetters());
	}

	IEnumerator ShowLetters() {

		for (int i = 0; i < alphaListPrefabs.Count; i++) {
			alphaListPrefabs[i].Show();
			yield return new WaitForSeconds(0.2f);
		}
	}

}

[System.Serializable]
public class PositionParam {
	public List<Vector3> positions;
}

