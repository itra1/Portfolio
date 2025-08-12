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
			OctopusController.Instance.UnSetLineCheck();
		}
	}

	public void Check() {
		if (Physics2D.Linecast(line.GetPosition(0), line.GetPosition(1), octopusMask)) {
			if (!_isCheck) {
				OctopusController.Instance.SetLineCheck();
			}
			_isCheck = true;
		} else {
			if (!_isCheck) return;
			if (_isCheck) {
				OctopusController.Instance.UnSetLineCheck();
			}
			_isCheck = false;
		}
	}
	
}
