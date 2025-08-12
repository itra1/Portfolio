using UnityEngine;
using System.Collections;
using Aura2API;

namespace Slate.ActionClips.Aura
{
  public abstract class AuraBase : ActorActionClip
  {

    [SerializeField]
    [HideInInspector]
    protected float _length = 1;
    public float targetValue;
    public EaseType interpolation = EaseType.QuadraticInOut;
    protected AuraVolume _aura;
    protected float newValue;
    protected float _startvalue;
    public override string info
    {
      get { return string.Format("Change aura emition {0}", targetValue); }
    }

    public override float length
    {
      get { return _length; }
      set { _length = value; }
    }

    public override float blendIn
    {
      get { return length; }
    }

	 protected override void OnEnter()
	 {
      _aura = actor.GetComponent<AuraVolume>();
      SetStartValue();
    }

    
	 protected override void OnUpdate(float deltaTime)
	 {
      newValue = Easing.Ease(interpolation, _startvalue, targetValue, deltaTime / length);
      ConfirmValue();
    }

    protected abstract void ConfirmValue();
    protected abstract void SetStartValue();

  }
}