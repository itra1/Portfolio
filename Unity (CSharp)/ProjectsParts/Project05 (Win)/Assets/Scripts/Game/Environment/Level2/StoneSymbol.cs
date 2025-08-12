using UnityEngine;
using System.Collections;
using UnityEngine.VFX;
using DG.Tweening;

namespace it.Game.Environment.Level2
{
  public class StoneSymbol : Environment
  {
	 /*
     * Состояния
     * 0 - не ипользовано
     * 1 - использовано
     * 
     */

	 [SerializeField]
	 private VisualEffect _visualEffect;

    [SerializeField]
    private Light _light;

	 public void Use()
	 {
      State = 1;
      ConfirmState();
      Save();
	 }

    protected override void ConfirmState(bool isForce = false)
    {
      base.ConfirmState(isForce);

      if (isForce)
      {
        if(State == 1)
        {
          _light.intensity = 0;
          _visualEffect.Stop();
        }
        else
        {
          _light.intensity = 1.78f;
          _visualEffect.Play();
        }
      }
      else
      {
        if(State == 1)
        {
          DOTween.To(()=>  _light.intensity ,(x)=>  _light.intensity = x ,0,1f);
          _visualEffect.Stop();
        }
      }

    }


  }
}