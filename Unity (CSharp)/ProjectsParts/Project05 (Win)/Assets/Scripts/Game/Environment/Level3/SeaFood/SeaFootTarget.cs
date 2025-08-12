using UnityEngine;
using System.Collections;
namespace it.Game.Environment.Level3
{
  public class SeaFootTarget : MonoBehaviourBase, it.Game.Player.Interactions.IInteractionCondition
  {
    [SerializeField]
    private int _index;
    [SerializeField]
    private SeaFoodController _controller;
    [SerializeField]
    private SeaFoodItem _food;

    public bool InteractionReady()
    {
      return _controller.CheckInterractItemReady(_index);
    }

    public void VisibleFood(bool isVisible, bool isForce = false)
    {
      if (!isVisible)
      {
        _food.gameObject.SetActive(false);
        return;
      }

      if (!isForce)
        _food.ColorShow(null);
      else
        _food.gameObject.SetActive(true);
    }

    public void Interruct()
    {
      if (!InteractionReady())
        return;
      _controller.ShowItem(_index);
    }
  }
}