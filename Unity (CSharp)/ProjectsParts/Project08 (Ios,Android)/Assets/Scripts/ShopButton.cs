using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour {

	public GameObject[] deactiveObject;

	private void OnEnable() {
		BillingManager.OnChangeStatus += ChangeStatus;
		ChangeStatus();
	}

	private void OnDisable() {
		BillingManager.OnChangeStatus -= ChangeStatus;
	}
	
	private void ChangeStatus() {
		GetComponent<Button>().interactable = !BillingManager.Instance.isProcessBye;
		for (int i = 0; i < deactiveObject.Length; i++) {
			deactiveObject[i].SetActive(BillingManager.Instance.isProcessBye);
		}
	}

}
