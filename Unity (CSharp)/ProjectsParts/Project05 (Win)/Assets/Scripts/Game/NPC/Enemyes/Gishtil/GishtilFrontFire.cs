using UnityEngine;
using DG.Tweening;
using DigitalRuby.ThunderAndLightning;
using it.Game.NPC.Enemyes.Boses.Hunter;
using System.Collections;
using com.ootii.Geometry;
using it.Game.Player;

namespace it.Game.Environment.Level6.Gishtil
{
  /// <summary>
  /// Поток огня
  /// </summary>
  public class GishtilFrontFire : MonoBehaviourBase
  {
	 [SerializeField]
	 private ParticleSystem[] _particleSystem;
	 [SerializeField]
	 private Light _lightHand;
	 private float _lightHandIntencity;
	 [SerializeField]
	 private Light _lightMiddle;
	 private float _lightMiddleIntencity;
	 [SerializeField]
	 private Light _lightTarget;
	 private float _lightTargetIntencity;

	 private bool _isPlayerDamage;

	 private bool _isActive;

	 private void Start()
	 {
		Deactivate();
		_lightHandIntencity = _lightHand.intensity;
		_lightMiddleIntencity = _lightMiddle.intensity;
		_lightTargetIntencity = _lightTarget.intensity;
		_lightHand.intensity = 0;
		_lightMiddle.intensity = 0;
		_lightTarget.intensity = 0;
	 }

	 public void Activate()
	 {
		_isPlayerDamage = false;

		DOVirtual.DelayedCall(0.5f,()=>{

		  _lightHand.DOIntensity(_lightHandIntencity, 0.5f);
		  _lightMiddle.DOIntensity(_lightMiddleIntencity, 0.5f).SetDelay(0.3f);
		  _lightTarget.DOIntensity(_lightTargetIntencity, 0.5f).SetDelay(0.6f);

		  ParticleActivate(true);
		  DOVirtual.DelayedCall(0.7f, () =>
		  {
			 _isActive = true;
		  });
		});

	 }

	 public void Deactivate()
	 {
		_isActive = false;
		ParticleActivate(false);
		_lightHand.DOIntensity(0, 0.5f);
		_lightMiddle.DOIntensity(0, 0.5f).SetDelay(0.3f);
		_lightTarget.DOIntensity(0, 0.5f).SetDelay(0.6f);
		DOVirtual.DelayedCall(0.5f, () =>
		{
		  _isActive = true;
		});
	 }

	 private void ParticleActivate(bool isActive)
	 {
		for(int i = 0; i < _particleSystem.Length; i++)
		{
		  var emis =  _particleSystem[i].emission;
		  emis.enabled = isActive;
		}
	 }

	 private void LateUpdate()
	 {
		if (!_isActive)
		  return;
		RayCast();
	 }
	 private void RayCast()
	 {
		RaycastHit _hit;
		Vector3 startSpawn = transform.position;

		if (RaycastExt.SafeRaycast(startSpawn, transform.forward, out _hit, 8, -1, transform))
		{
		  if (!_isPlayerDamage)
		  {
			 if (_hit.collider.GetComponent<PlayerBehaviour>() != null)
			 {
				Game.Managers.GameManager.Instance.UserManager.Health.Damage(this, 34);
				_isPlayerDamage = true;
			 }
		  }
		  var lighting = _hit.collider.GetComponentInParent<GishtilLighting>();

		  if (lighting != null)
		  {
			 lighting.FireCintact(true);
		  }

		}
	 }

  }
}