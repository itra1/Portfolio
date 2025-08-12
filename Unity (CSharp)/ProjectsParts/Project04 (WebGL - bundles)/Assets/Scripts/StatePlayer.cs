using System.Collections;
using System.Collections.Generic;
using Network.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class StatePlayer : MonoBehaviour {

	public TextMeshProUGUI title;
	public Color friendColor;
	public Color enemyColor;

	public RectTransform healtLine;
	private Vector2 _startDelta;
	private PlayerBehaviour _playerBehaviour;
	//public RectTransform manaLine;

	private RectTransform _rectTransform;
	private RectTransform rectTransform {
		get {
			if (_rectTransform == null) {
				_rectTransform = GetComponent<RectTransform>();
			}
			return _rectTransform;
		}
	}

	private SortingGroup _sortingGroup;
	private SortingGroup sortingGroup {
		get {
			if (_sortingGroup == null) {
				_sortingGroup = GetComponent<SortingGroup>();
			}
			return _sortingGroup;
		}
	}

	public void Init(bool isFriend, PlayerBehaviour playerBehaviour) {
		if(_startDelta == Vector2.zero)
			_startDelta = healtLine.sizeDelta;
		_playerBehaviour = playerBehaviour;
		title.text = playerBehaviour.playerInfo.login;
		if(playerBehaviour.playerInfo.hp_max == 0) {
			playerBehaviour.playerInfo.hp = 1;
			playerBehaviour.playerInfo.hp_max = 1;
		}
		healtLine.sizeDelta = new Vector2(playerBehaviour.playerInfo.hp/ playerBehaviour.playerInfo.hp_max * _startDelta.x, healtLine.sizeDelta.y);
		//manaLine.sizeDelta = new Vector2(playerinfo. / playerinfo.hp_max, healtLine.sizeDelta.y);
		title.color = isFriend ? friendColor : enemyColor;
		MoveToPlayer();
	}

	private void LateUpdate() {
		MoveToPlayer();
	}

	private void MoveToPlayer() {
		rectTransform.position = Camera.main.WorldToScreenPoint(_playerBehaviour.transform.position ) + new Vector3(0, 100,0);
		sortingGroup.sortingOrder = (int)((_playerBehaviour.transform.position - Camera.main.transform.position).magnitude * -100);
	}


}
