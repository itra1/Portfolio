using UnityEngine;
using System.Collections;
namespace it.Game.Player.Interactions
{
  public interface IInteractionConditionHoldItem
  {
    /// <summary>
    /// Необходимый предмет для удержания
    /// </summary>
    /// <returns></returns>
    string NeedHoldItem();
  }
}