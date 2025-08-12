using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Company.Live {
  public class Heart: LiveCompany {

    public override float maxValue {
      get { return 5; }
    }

    public override float oneRunPrice {
      get { return 1; }
    }

    protected override float secondRepeat {
      get { return 3600; }
    }

  }
}