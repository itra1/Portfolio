using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUiTimer : MonoBehaviour {

  [SerializeField]
	private TextMeshProUGUI timerText;
	private float _oldSecond = -1;
  private float _minut;
  private float _second;

  public void ChangeTime(float timeFarm) {

    _minut = Mathf.Floor(timeFarm / 60);
    _second = Mathf.Floor(timeFarm - _minut * 60);
		if (_second == _oldSecond) return;
		_oldSecond = _second;

	  timerText.text = string.Format("{0:00}:{1:00}", _minut, _second);
	}


  private void OnEnable()
  {
    BattleManager.battleTimer += ChangeTime;
    ChangeTime(0);
  }

  private void OnDisable()
  {
    BattleManager.battleTimer -= ChangeTime;
  }

}
