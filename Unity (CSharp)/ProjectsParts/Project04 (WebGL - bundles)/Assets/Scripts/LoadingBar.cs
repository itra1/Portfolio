using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExEvent;
using UnityEngine.UI;

public class LoadingBar : EventBehaviour {

	public RectTransform progressMask;
	public Text info;

	private float _maxValue;

	protected override void Awake() {
		base.Awake();
		_maxValue = progressMask.sizeDelta.x;
	}

	[ExEventHandler(typeof(LoadEvents.LoadProgress))]
	public void ChangeValues(LoadEvents.LoadProgress loadProgress) {
		info.text = loadProgress.info;
		progressMask.sizeDelta = new Vector2(_maxValue * (1 - loadProgress.progressValue), progressMask.sizeDelta.y);
	}

}
