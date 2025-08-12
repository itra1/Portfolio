using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace it.Game.Environment.GreenCthulhu
{
  public class HasturMiniInteraction : MonoBehaviourBase, it.Game.Player.Interactions.IInteractionCondition
  {

    [SerializeField]
    private HasturMini _hastur;

    public bool InteractionReady()
    {
      return _hastur.InteractionReady();
    }
  }
}