using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;

namespace it.Game.Environment.Level3.PuzzleGate
{
  public class KeyBlock : MonoBehaviourBase, Player.Interactions.IInteractionCondition
  {
    [SerializeField]
    private PuzzleGate gate;
    public bool InteractionReady()
    {
      return gate.InteractionReady();
    }
  }
}