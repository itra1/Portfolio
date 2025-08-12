using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Configuration.Survivles {

  public class EnemyPrice {

    public float azteck;
    public float azteckForward;
    public float boomerang;
    public int balls;
    public string distantion;
    public float empty;
    public float fatAzteck;
    public float gigant;
    public float handingAzteck;
    public float spearAzteck;
    public float summary;
    public float dist {
      get {

        float res = 0;

        if (float.TryParse(distantion, out res)) {
          return res;
        } else {
          return 0;
        }
      }
    }

  }
}