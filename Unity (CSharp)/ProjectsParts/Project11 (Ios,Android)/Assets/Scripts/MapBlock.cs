using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : MonoBehaviour {

  public SpriteRenderer back;
  public SpriteRenderer road;
  public BezierCurve bezier;
  
  public List<Transform> rockPointList;


  public float SetGraphic(MapGraphicLibrary mgl, int number, bool isTransaction = false) {

    road.sprite = isTransaction ? mgl.transitionRoad :  mgl.roads[number];
    back.sprite = isTransaction ? mgl.transitionBack : mgl.backGround;

    float roadSizeY = road.sprite.rect.height / road.sprite.pixelsPerUnit;
    float backSizeY = back.sprite.rect.height / back.sprite.pixelsPerUnit;
    back.transform.localScale = new Vector3(1, roadSizeY / backSizeY, 1);


    return roadSizeY;
  }


  public void Inite(MapGraphicLibrary mgl) {

    rockPointList.ForEach(elem => {

      Sprite spr = mgl.maxDecor[ Random.Range( 0, mgl.maxDecor.Count)];

      GameObject inst = new GameObject();

      if (elem.transform.position.x < 0) {
        inst.transform.localScale = Vector3.one;
      } else {
        inst.transform.localScale = new Vector3(-1, 1, 1);
      }

      inst.transform.SetParent( MapManager.Instance.bigDecorParent);
      inst.transform.position = elem.transform.position;
      SpriteRenderer sr = inst.AddComponent<SpriteRenderer>();
      sr.sprite = spr;
      sr.sortingOrder = 30000 - (int)(inst.transform.localPosition.y* 100);

    });

  }
  
}
