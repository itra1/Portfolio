using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Challenges.KingGameOfUr
{
  public class ActivatorCondition : MonoBehaviourBase, it.Game.Player.Interactions.IInteractionCondition
  {
    [SerializeField]
    private KingGameOfUr _game;
    [SerializeField]
    private int _targetState;

    public bool InteractionReady()
    {
      return _game.State == _targetState;
    }
  }
}