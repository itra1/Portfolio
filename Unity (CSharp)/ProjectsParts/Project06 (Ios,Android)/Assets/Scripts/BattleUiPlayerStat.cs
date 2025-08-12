using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.User;

public class BattleUiPlayerStat : ExEvent.EventBehaviour {

	private AudioPoint _hearthClipPoint;

	private void OnEnable() {
    //UserHealth.OnChange += LiveChange;
		LiveChange(UserHealth.Instance.Value, UserHealth.Instance.Max);
	}

	private void OnDisable() {
	  //UserHealth.OnChange -= LiveChange;
	}
	
	#region Live
	public RectTransform liveLine;

	void LiveChange(float liveNow, float liveMax) {
		//liveLine.localPosition = new Vector3(liveLine.localScale.x, -35.5f + (70 * (Mathf.Max(liveNow, 0) / liveMax)), liveLine.localScale.z);
		//liveLine.localScale = new Vector3(Mathf.Max(liveNow, 0) / liveStart, liveLine.localScale.y, liveLine.localScale.z);
		CorrectHearthAudio(liveMax, liveNow);
	}


	void CorrectHearthAudio(float liveStart, float liveNow) {
		if (_hearthClipPoint == null) return;
		if (liveNow / liveStart >= 0.7f)
			_hearthClipPoint.source.volume = 0;
		else
			_hearthClipPoint.source.volume = 1 - (liveNow / (liveStart * 0.7f));
	}

	#endregion


}
