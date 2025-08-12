using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Slate.ActionClips;
using UnityEngine.VFX;

namespace Slate.ActionClips.VFX
{
  [Category("VFX")]
  public abstract class VFXBaseClip : ActorActionClip<UnityEngine.VFX.VisualEffect>
  {

    protected bool HasParameter(string name)
    {
      if (actor == null)
      {
        return false;
      }

      if (!actor.HasFloat(name))
        return false;

      return true;

    }

  }
}