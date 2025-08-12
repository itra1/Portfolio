using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyBonusGift : MonoBehaviour {

	public Action<int> OnClick;
	private int _num;
	public Animation anim;

	public void Set(int num,Transform parent, Action<int> OnSelect) {
		_num = num;
		GetComponent<Follower>().source = parent;
		OnClick = OnSelect;

		int numanim = UnityEngine.Random.Range(1, 5);

		anim.Play("Alpha" + numanim);
	}

	public void Click() {
		if (OnClick != null) OnClick(_num);
	}

}
