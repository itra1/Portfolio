using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHeartUi : MonoBehaviour {

	public GameObject activeIcon;
	public GameObject deactiveIcon;

	private bool _isActive;

	public bool isActive {
		get { return _isActive; }
		set {
			_isActive = value;
			//TODO Анимация Исчезновения сердца
			activeIcon.SetActive(_isActive);
		}
	}


}
