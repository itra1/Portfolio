using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using Aura2API;
using DG.Tweening;

namespace it.Game.Managers
{
  public class AuraManager : Singleton<AuraManager>
  {
	 [SerializeField]
	 private AuraVolume _form2Volume;
	 public AuraVolume Form2Volume { get => _form2Volume; set => _form2Volume = value; }

	 private void Start()
	 {
		_form2Volume.enabled = false;
	 }

	 public void ActivateForm2(
		UnityEngine.Events.UnityAction OnMiddle, 
		UnityEngine.Events.UnityAction OnComplete, 
		float duration = 0.1f, 
		float middlePause = 0)
	 {
		_form2Volume.densityInjection.strength = 3;
		_form2Volume.enabled = true;
		DOTween.To(() => _form2Volume.densityInjection.strength, (x) => _form2Volume.densityInjection.strength = x, 100, duration).OnComplete(() =>
		{
		  OnMiddle?.Invoke();
		  DOTween.To(() => _form2Volume.densityInjection.strength, (x) => _form2Volume.densityInjection.strength = x, 3, duration).OnComplete(() =>
		  {
			 OnComplete?.Invoke();
		  }).SetDelay(middlePause);
		});
	 }

	 public void DeactivateForm2()
	 {

		_form2Volume.densityInjection.strength = 3;
		_form2Volume.enabled = false;
	 }

  }
}