using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace HutongGames.PlayMaker
{
  public class FsmVisualEffect : NamedVariable
  {
    private VisualEffect _effect;

    public FsmVisualEffect()
    {
      _effect = null;
    }
    public FsmVisualEffect(FsmVisualEffect source) {

      this.Value = source.Value;
    }
    public VisualEffect Value { get => _effect; set => _effect = value; }
  }
}