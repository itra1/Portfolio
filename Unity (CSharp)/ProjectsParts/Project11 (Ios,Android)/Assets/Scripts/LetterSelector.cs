using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterSelector : ExEvent.EventBehaviour {

	public OctopusChecker linePrefab;
	public List<OctopusChecker> instanceLineList = new List<OctopusChecker>();
	public List<OctopusChecker> showLineList = new List<OctopusChecker>();

	private OctopusChecker _activeLine;

	private Vector3 startPos;
	private Vector3 endPos;

	public OctopusChecker GetInstance() {
		OctopusChecker lr = instanceLineList.Find(x => !x.gameObject.activeInHierarchy);
		if (lr == null) {
			GameObject inst = Instantiate(linePrefab.gameObject) as GameObject;
			inst.transform.SetParent(transform);
			lr = inst.GetComponent<OctopusChecker>();
			instanceLineList.Add(lr);
		}
		return lr;
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeGamePhase))]
	public void OnEndGame(ExEvent.GameEvents.OnChangeGamePhase eg) {
		if (eg.last == GamePhase.game && eg.next != GamePhase.game && showLineList.Count > 0) {
			HideAll();
			_activeLine = null;
		}
	}

	public void StartDrag(Vector3 pos) {
		/*
		startPos = pos;
		endPos = pos;
		_activeLine = GetInstance();
		_activeLine.gameObject.SetActive(true);
		showLineList.Add(_activeLine);
		_activeLine.numPositions = 2;
		_activeLine.SetPositions(new Vector3[]{startPos, endPos});
		*/
	}

	public void ChangePosition(Vector3 pos) {
		endPos = pos;
		if (_activeLine != null) {
			_activeLine.line.SetPositions(new Vector3[] { startPos, endPos });
			_activeLine.Check();
		}
	}

	public void AddPoint(Vector3 pos) {
		PlayWordClickAudio();
		endPos = pos;
		if (_activeLine != null) {
			_activeLine.line.SetPositions(new Vector3[] { startPos, endPos });
			_activeLine.Check();
		}
		_activeLine = GetInstance();
		_activeLine.gameObject.SetActive(true);
		showLineList.Add(_activeLine);
		_activeLine.line.positionCount = 2;
		startPos = pos;
		_activeLine.transform.position = startPos;
		_activeLine.Check();
		_activeLine.line.SetPositions(new Vector3[] { startPos, endPos });
	}

	public void TrimList(int num) {

		while (showLineList.Count > num) {
			showLineList[showLineList.Count-1].gameObject.SetActive(false);
			showLineList.RemoveAt(showLineList.Count - 1);
			_activeLine = showLineList[showLineList.Count - 1];
			_activeLine.Check();
			startPos = _activeLine.line.GetPosition(0);
			endPos = _activeLine.line.GetPosition(1);
		}

	}

	public void HideAll() {
		showLineList.ForEach(x=>x.gameObject.SetActive(false));
		showLineList.Clear();
	}

	public List<AudioClipData> letteClickAudio;
	public void PlayWordClickAudio() {
		AudioManager.PlayEffects(letteClickAudio[Random.Range(0, letteClickAudio.Count)], AudioMixerTypes.effectUi);
	}

}
