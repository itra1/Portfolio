using UnityEngine;
using System.Collections;

namespace Slate.ActionClips.Particles.Transforms
{

  [Category("Transform")]
  [Description("Translate the actor by specified value and optionaly per second")]
  public class TranslateForvard : ActorActionClip
  {

    [SerializeField]
    [HideInInspector]
    private float _length = 1;
    public float speed = 1;
    public EaseType interpolation = EaseType.QuadraticInOut;

    private Vector3 originalPos;

    public override string info
    {
      get { return string.Format("Translate forvard wish speed {0} ", speed); }
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
      originalPos = actor.transform.localPosition;
    }

    protected override void OnUpdate(float deltaTime)
    {
      var target = originalPos + (actor.transform.forward * speed * length);
      actor.transform.localPosition = Easing.Ease(interpolation, originalPos, target, deltaTime / length);
    }

    protected override void OnReverse()
    {
      actor.transform.localPosition = originalPos;
    }
  }
}