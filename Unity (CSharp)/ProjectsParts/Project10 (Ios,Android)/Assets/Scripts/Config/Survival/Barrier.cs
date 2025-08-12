using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Configuration.Survivles {
  public class Barrier {

    public string barrierProbablility;
    public float pit;
    public string distantion;
    public float doubleSkeleton;
    public float doubleStone;
    public float ghost;
    public float ghostGroup;
    public float handingBarrier;
    public float spiderNet;
    public float stone;
    public float stoneAndHanding;
    public float stoneSkeleton;
    public float summary;

    public float probablility {
      get {
        float res;
        if(float.TryParse(barrierProbablility, out res)) {
          return res;
        } else {
          return -1;
        }
      }
    }

    public FloatSpan dist {
      get {
        string[] strArr = distantion.Split(new char[] { '-' });

        FloatSpan spn = new FloatSpan();

        if (strArr.Length == 1) {
          float res = 0;
          if (float.TryParse(strArr[0], out res))
            spn.max = res;
        } else {

          float res = 0;
          if (float.TryParse(strArr[0], out res))
            spn.min = res;
          if (float.TryParse(strArr[1], out res))
            spn.max = res;
        }

        return spn;
      }
    }

  }
}
