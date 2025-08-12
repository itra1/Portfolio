using UnityEngine;
using DG.Tweening;
using DigitalRuby.ThunderAndLightning;
using it.Game.NPC.Enemyes.Boses.Hunter;
using System.Collections;

namespace it.Game.Environment.Level6.Gishtil
{
  /// <summary>
  /// Тьма
  /// </summary>
  public class GishtilShadowCaster : MonoBehaviourBase
  {
	 [SerializeField]
	 private Aura2API.AuraVolume _aura;
	 private bool _isActive;
	 private float _timeActivate;
	 private float _timeActive;
	 private it.Game.NPC.Enemyes.Boses.Hunter.Gishtil _gishtil;

	 [SerializeField]
	 private ParticleSystem[] _particles;

	 private void Start()
	 {
		EmissionActivate(false);
	 }

	 public void Activate(float timeActive, it.Game.NPC.Enemyes.Boses.Hunter.Gishtil gishtil)
	 {
		_aura.Reinitialize();
		_gishtil = gishtil;
		_timeActive = timeActive;

		DOVirtual.DelayedCall(3,()=>{
		  _gishtil.SetShow(false);
		});

		StartTweenChange(7);
		DOVirtual.DelayedCall(8, () =>
		{
		  StartTweenChange(0);
		});
	 }
	 public void OnReset()
	 {
		_aura.densityInjection.strength = 0;
	 }

	 private void EmissionActivate(bool isEnable)
	 {

		for (int i = 0; i < _particles.Length; i++)
		{
		  var emis = _particles[i].emission;
		  emis.enabled = isEnable;
		}
	 }


	 private void StartTweenChange(float target)
	 {
		StartCoroutine(ShadowSet(target));
	 }

	 IEnumerator ShadowSet(float value)
	 {
		float target = value;
		float source = _aura.densityInjection.strength;

		EmissionActivate(value > 0);
		yield return new WaitForSeconds(2);

		float startVal = 0;
		if(value <= 0)
		{
		  _gishtil.SetShow(true);
		}

		while (startVal < 1)
		{
		  startVal += Time.deltaTime*0.2f;
		  _aura.densityInjection.strength = Mathf.Lerp(source, target, startVal);

		  yield return null;
		}
		_aura.densityInjection.strength = Mathf.Lerp(source, target, 1);
	 }
  }
}