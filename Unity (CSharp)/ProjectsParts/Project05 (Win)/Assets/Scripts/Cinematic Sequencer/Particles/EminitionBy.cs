using UnityEngine;
using System.Collections;

namespace Slate.ActionClips.Particles
{
  [Category("Emitions")]
  [Description("Установка целевого значения емишен у партикле системс")]
  public class EminitionBy : ActorActionClip
  {

    [SerializeField]
    [HideInInspector]
    private float _length = 1;

    public float targetEmitions;
    public EaseType interpolation = EaseType.QuadraticInOut;

    private ParticleSystem _particles;
    ParticleSystem.EmissionModule _emitionModule;
    private ParticleSystem.MinMaxCurve _emitionCurve;
    private float originalEmitions;

    public override string info
    {
      get { return string.Format("Change emition {0}", targetEmitions); }
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
      _particles = actor.GetComponent<ParticleSystem>();
      _emitionModule = _particles.emission;
      _emitionCurve = _particles.emission.rateOverTime;
      originalEmitions = _emitionCurve.constant;
    }

    protected override void OnUpdate(float deltaTime)
    {
      float constantValue = Easing.Ease(interpolation, originalEmitions, targetEmitions, deltaTime / length);

      _emitionCurve.constant = constantValue;
      _emitionModule.rateOverTime = _emitionCurve;
    }

    protected override void OnReverse()
    {
      _emitionCurve.constant = originalEmitions;
      _emitionModule.rateOverTime = _emitionCurve;
    }



  }
}