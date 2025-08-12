using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBonusLine : MonoBehaviour {

  public Timer timer;

	// Use this for initialization
	void Start () {
    timer.StartTimer(Shop.Instance.dateLastBye.AddDays(3) - System.DateTime.Now);

  }
	
}
