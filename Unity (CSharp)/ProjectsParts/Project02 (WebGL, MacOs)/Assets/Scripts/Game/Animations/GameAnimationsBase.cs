using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Garilla.Games.Animations
{
  public class GameAnimationsBase
  {
    public UnityAction OnComplete;

    public TableAnimationsManager AnimationManager;

    protected Hashtable _hash;
    public virtual void Play(){

		}

    public virtual void Init(Hashtable hashtable)
    {

		}

  }
}