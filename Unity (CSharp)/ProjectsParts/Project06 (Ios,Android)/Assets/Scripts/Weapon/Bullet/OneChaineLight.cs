using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneChaineLight : MonoBehaviour {
	private int elementNum = 0;
	public float distanceDef;
	public Transform parentGraphic;

	public List<GameObject> lightList;

	public bool useTime = true;

	private void OnEnable() {
		
		elementNum = 0;

		timeEnd = Time.time + timeWork;
		Reposition();
	}
	
	private void Update() {
		elementNum++;
		for (int i = 0; i < lightList.Count; i++) lightList[i].SetActive(i == elementNum % lightList.Count);

		Reposition();

		if (useTime && Time.time > timeEnd) gameObject.SetActive(false);
	}

	private void Reposition() {
		if (start == null) return;

		Vector3 vectorTarget = end.position - start.position;
		float koeff = vectorTarget.magnitude / 5;

		transform.localScale = new Vector3(koeff, koeff, koeff);
		float angle = Vector3.Angle(Vector2.up, vectorTarget);
		transform.eulerAngles = new Vector3(0, 0, -angle);
		transform.position = start.position + vectorTarget / 2 + Vector3.up;
	}

	private Transform start;
	private Transform end;
	private float timeWork;
	private float timeEnd;

	public void SetLight(Transform start, Transform end, float time) {
		this.start = start;
		this.end = end;
		this.timeWork = time;
	}

}