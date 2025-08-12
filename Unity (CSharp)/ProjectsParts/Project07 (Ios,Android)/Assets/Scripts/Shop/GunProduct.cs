using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProduct : ProductBase {

	public string guidType;


	public override bool Buy() {

		if (!CheckCash()) return false;
		ChangeCash();
    return true;
  }


}
