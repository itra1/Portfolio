using ExEvent;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Помошник в работе sfx эффектов
/// </summary>
public class sfx: ExEvent.EventBehaviour, ISpriteOrder
{

  public List<ParticleSystem> partiList;

  /// <summary>
  /// Родитель, если необходимо отключать его
  /// </summary>
  [SerializeField] GameObject parentObject;

  /// <summary>
  /// Время жизни объекта, если требуется его деактивация по таймеру
  /// </summary>
  [SerializeField]
  float timer;

  public bool isDeactive = true;

  void OnEnable()
  {
    if (timer > 0)
      Invoke("DeactiveObject", timer);

    SpriteRenderer[] spriteArr = GetComponentsInChildren<SpriteRenderer>();

    foreach (var spriteArrElem in spriteArr)
    {
      spriteArrElem.sortingOrder = 10000;

    }

  }

  [ExEvent.ExEventHandler(typeof(BattleEvents.StartBattle))]
  public void BattleStart(BattleEvents.StartBattle startEvent)
  {
    gameObject.SetActive(false);
  }

  private void OnDisable()
  {
    CancelInvoke("DeactiveObject");
  }

  public int spriteOrderValue { get; set; }
  public void SetSpriteOrder(int order)
  {
    spriteOrderValue = order;
    partiList.ForEach(x => x.GetComponent<Renderer>().sortingOrder = spriteOrderValue);
    //if (skeletonAnimation != null)
    //	skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = order;
  }

  /// <summary>
  /// Деактивация объекта
  /// </summary>
  public void DeactiveObject()
  {

    if (!isDeactive)
      return;

    if (parentObject != null)
      parentObject.SetActive(false);
    else
      gameObject.SetActive(false);
  }
}
