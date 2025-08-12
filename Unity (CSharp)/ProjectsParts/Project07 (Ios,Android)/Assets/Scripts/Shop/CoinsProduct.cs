using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsProduct : ProductBase {

  public int count;

  public override bool Buy() {

    if (!CheckCash())
      return false;
    ChangeCash();

    UserManager.Instance.AddAction(() =>
    {
      UserManager.Instance.Gold += count;
    });
    return true;
  }

}
