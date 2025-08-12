using UnityEngine;
using System;
using System.Collections.Generic;

public struct MoveObjecsElem {
  public int id;
  public Vector2 position;
  public Vector2 velocity;
}

public class MoveObjects : Packet {

  List<MoveObjecsElem> moveList = new List<MoveObjecsElem>();
  public static event Action<MoveObjecsElem> OnMove;

  public override void ReadImpl() {

    moveList.Clear();

    int count = ReadH();
    for(int i = 0; i < count; i++) {
      MoveObjecsElem elem = new MoveObjecsElem();
      elem.id = ReadD();
      elem.position = MapManager.PositionInvers(ReadVec2());
      elem.velocity = MapManager.VectorInvers(ReadVec2());
      moveList.Add(elem);
    }
  }

  public override void Process() {

    foreach(MoveObjecsElem elem in moveList) {
      //Debug.Log(elem.position);
      if(OnMove != null) OnMove(elem);
    }

  }

}
