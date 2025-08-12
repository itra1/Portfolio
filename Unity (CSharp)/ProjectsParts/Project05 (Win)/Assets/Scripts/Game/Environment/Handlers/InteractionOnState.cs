using UnityEngine;
using System.Collections;
namespace it.Game.Environment.Handlers
{
  public class InteractionOnState : MonoBehaviour, it.Game.Player.Interactions.IInteractionCondition
  {

    [SerializeField]
    private TypeCompare _compare;

    [SerializeField]
    private Environment _environment;

    [SerializeField]
    private int _state = 0;

    public bool InteractionReady()
    {
      if (_environment == null)
        return false;

      switch (_compare)
      {
        case TypeCompare.more:
          return _environment.State > _state;
        case TypeCompare.less:
          return _environment.State < _state;
        case TypeCompare.equal:
          return _environment.State == _state;
      }

      return false;
    }

    public enum TypeCompare
    {
      equal, more, less
    }
  }
}