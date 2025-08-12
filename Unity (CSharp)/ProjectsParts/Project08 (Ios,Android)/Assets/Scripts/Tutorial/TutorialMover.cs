using System;
using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

public class TutorialMover : MonoBehaviour {

	public Action OnMoveComplete;

	public Animation animComp;

	private List<Vector3> _positionList;
	private Vector3 _nextTarget;

	private bool isMove = false;

	public float speed;
	public GameObject graphic;

	public LineRenderer lineRender;

	private List<Vector3> pointList = new List<Vector3>();
	
	public void Play(List<Vector3> moverList) {
		_positionList = moverList;

		

		if (isMove) {
			Stop();
			return;
		}

		PlayMove();

	}

	private void StartColor() {
		for (int i = 0; i < lineRender.colorGradient.alphaKeys.Length; i++)
			lineRender.colorGradient.alphaKeys[i].alpha = 0.5f;
	}

	private void AddPoint() {
		pointList[pointList.Count - 1] = transform.position;
		pointList.Add(transform.position);
		lineRender.positionCount = pointList.Count;
		lineRender.SetPositions(pointList.ToArray());
		//lineRender.
	}

	private void PlayMove() {
		
		_nextTarget = _positionList[0];
		_positionList.RemoveAt(0);
		transform.position = _nextTarget;
		_nextTarget = _positionList[0];
		_positionList.RemoveAt(0);


		pointList.Clear();
		pointList.Add(transform.position);
		pointList.Add(transform.position);
		lineRender.positionCount = pointList.Count;
		lineRender.SetPositions(pointList.ToArray());

		StartColor();


		graphic.SetActive(true);
		animComp.Play("show");
	}

	public void ShowComplete() {
		isMove = true;
	}

	public void HideComplete() {
		Invoke("Next",1);
	}

	private void Next() {
		if (OnMoveComplete != null) OnMoveComplete();
	}

	public void Stop() {
		if (!isMove) return;
		isMove = false;
		pointList.Clear();
		lineRender.positionCount = pointList.Count;
		animComp.Play("hide");
		StartCoroutine(TranslateHide());
	}

	IEnumerator TranslateHide() {
		Gradient gr = lineRender.colorGradient;
		float val = gr.alphaKeys[0].alpha;
		
		while (val > 0) {
			if (val < 0) val = 0;
			val -= 1 * Time.deltaTime;
			for (int i = 0; i < gr.alphaKeys.Length; i++) {
				GradientAlphaKey gak = gr.alphaKeys[i];
				gak.alpha = val;
				gr.alphaKeys[i] = gak;
			}
			yield return null;
		}
	}

	private void Update() {

		if (!isMove) return;

		Vector3 newPosition = transform.position + (_nextTarget - transform.position).normalized * Time.deltaTime * speed;

		if ((newPosition - transform.position).magnitude < (_nextTarget - transform.position).magnitude) {
			transform.position = newPosition;
			lineRender.SetPosition(pointList.Count-1,transform.position);
			return;
		}

		transform.position = _nextTarget;
		AddPoint();
		lineRender.SetPosition(pointList.Count - 1, transform.position);

		if (_positionList.Count == 0) {
			Stop();
			return;
		}

		_nextTarget = _positionList[0];
		_positionList.RemoveAt(0);

	}
	
}
