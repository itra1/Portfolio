using UnityEngine;
using System.Collections;

/// <summary>
/// Сортировка объектов на сцене
/// </summary>
public class SortingLayerController : MonoBehaviour {

  /// <summary>
  /// Применить сортировку
  /// </summary>
  /// <param name="layerId">Идентификатор слоя</param>
  /// <param name="orderLayer">Номер сортировки</param>
  public void SetSortingLayer(int layerId, int orderLayer) {

    SpriteRenderer sprite = GetComponent<SpriteRenderer>();

    if(sprite != null) {
      sprite.sortingLayerID = layerId;
      sprite.sortingOrder = orderLayer;
    }

    MeshRenderer mesh = GetComponent<MeshRenderer>();

    if(mesh != null) {
      mesh.sortingLayerID = layerId;
      mesh.sortingOrder = orderLayer;
    }
    
  }
}
