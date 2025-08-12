using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProductBase  : MonoBehaviour {

	public string title;

	public float softCashPrise;
	public float hardCashPrise;
	public Sprite icon;

	public abstract bool Buy();
	public virtual bool CheckCash() {
		return softCashPrise <= UserManager.Instance.Gold && hardCashPrise <= UserManager.Instance.HardCash;
	}

	public virtual void ChangeCash() {
		UserManager.Instance.Gold -= (int)softCashPrise;
		UserManager.Instance.HardCash -= (int)hardCashPrise;
	}

}
