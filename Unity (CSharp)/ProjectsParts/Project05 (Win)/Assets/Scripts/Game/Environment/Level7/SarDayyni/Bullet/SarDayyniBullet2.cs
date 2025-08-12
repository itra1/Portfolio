using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace it.Game.Environment.Level7.SarDayyni
{
  public class SarDayyniBullet2 : MonoBehaviour
  {
	 public Transform _collider;
	 public ParticleSystem particle;

	 private void OnEnable()
	 {
		particle.gameObject.SetActive(false);
		EmiterParticles(false);
		particle.gameObject.SetActive(true);
		EmiterParticles(true);
		_collider.localPosition = new Vector3(0, -22, 0);
		_collider.DOLocalMoveY(60, 13);

		DOVirtual.DelayedCall(4, () =>
		{
		  EmiterParticles(false);
		});
		DOVirtual.DelayedCall(16, () =>
		{
		  gameObject.SetActive(false);
		});
	 }


	 private void EmiterParticles(bool isEmit)
	 {
		ParticleSystem[] particles = particle.GetComponentsInChildren<ParticleSystem>();
		for(int i = 0; i < particles.Length; i++)
		{
		  var emis = particles[i].emission;
		  emis.enabled = isEmit;
		}
	 }
  }
}