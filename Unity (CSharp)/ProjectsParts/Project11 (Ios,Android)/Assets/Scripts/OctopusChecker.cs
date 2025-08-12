using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusChecker : MonoBehaviour {

	public LayerMask octopusMask;
	public LineRenderer line;

	private bool _isCheck;
	
	private void OnDisable() {
		if (_isCheck) {
			_isCheck = false;
		}
	}

	public void Check() {
		if (Physics2D.Linecast(line.GetPosition(0), line.GetPosition(1), octopusMask)) {
			if (!_isCheck) {
			}
			_isCheck = true;
		} else {
			if (!_isCheck) return;
			if (_isCheck) {
				
			}
			_isCheck = false;
		}
	}
	
}
