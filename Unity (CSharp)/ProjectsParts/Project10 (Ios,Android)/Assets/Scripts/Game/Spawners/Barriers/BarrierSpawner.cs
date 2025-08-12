using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BarrierSpawner: Singleton<BarrierSpawner> {
  
  public virtual void CalcNextGenerate() {  }

}
