using UnityEngine;
using System;
using System.Collections.Generic;

namespace Company.Live {

  public class Energy: LiveCompany {

    public override float maxValue {
      get { return 100; }
    }

    public override float oneRunPrice {
      get { return 10; }
    }

    protected override float secondRepeat {
      get { return 3600; }
    }

  }
}