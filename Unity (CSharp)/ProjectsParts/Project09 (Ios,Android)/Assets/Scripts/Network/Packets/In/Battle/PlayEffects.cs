using UnityEngine;
using System.Collections.Generic;

public class PlayEffects : Packet {

  public static event Actione<int,List<EffectData>> OnEffect;
  
  int ownerId;

  public struct EffectData {
    public EffectType effectType;
    public float time;
  }

  List<EffectData> effectData = new List<EffectData>();

  public override void ReadImpl() {
    ownerId = ReadD();
    int arrSize = ReadC();
    for(int i = 0; i < arrSize; i++) {
      EffectData eff = new EffectData();
      eff.effectType = (EffectType)ReadC();
      eff.time = ReadF();
      effectData.Add(eff);
    }
  }

  public override void Process() {
    if(OnEffect != null) OnEffect(ownerId, effectData);
  }

}
