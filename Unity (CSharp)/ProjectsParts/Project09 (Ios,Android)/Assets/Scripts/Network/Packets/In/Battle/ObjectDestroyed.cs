using UnityEngine;
using System.Collections;

public class ObjectDestroyed : Packet {

  public static event Actione<int> OnDestroyObject;

  int objectId;

  public override void ReadImpl() {
    objectId = ReadD();
  }

  public override void Process() {
    if(OnDestroyObject != null) OnDestroyObject(objectId);
  }

}
