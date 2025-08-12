using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace it.Game.Environment.Level2
{
  public class Cocon : Environment
  {
	 [SerializeField]
	 private ParticleSystem _particleSys;
	 [SerializeField]
	 private Material _material;
	 [SerializeField]
	 [ColorUsage(false, true)]
	 private Color _lightColor;

	 [SerializeField]
	 private UnityEngine.Events.UnityEvent OnActivate;

	 [SerializeField]
	 private it.Game.NPC.Enemyes.Crab[] crabs;

	 public void Activate()
	 {
		if (State > 0)
		  return;

		State = 1;

		DOTween.To(() => _material.GetColor("_EmissionColor"), (x) => _material.SetColor("_EmissionColor", x), _lightColor, 1f).OnComplete(() =>
		{
		  Pegasus();
		});

		ConfirmState();
		Save();
	 }

	 private void PegasusComplete()
	 {
		_particleSys.Play();
		DOTween.To(() => _material.GetColor("_EmissionColor"), (x) => _material.SetColor("_EmissionColor", x), _lightColor, 1f).SetDelay(1).OnComplete(() =>
		{
		  for (int i = 0; i < crabs.Length; i++)
			 crabs[i].Attack();
		});
	 }

	 void Pegasus()
	 {
		var pegasus = GetComponentInChildren<it.Game.Environment.Handlers.PegasusController>(true);

		if (pegasus == null)
		{
		  OnActivate?.Invoke();
		  return;
		}

		pegasus.Activate(() =>
		{
		  DOVirtual.DelayedCall(0.5f, () =>
		  {
			 OnActivate?.Invoke();
			 DOVirtual.DelayedCall(3f, () =>
			 {
				pegasus.Deactivate();
				PegasusComplete();
			 });
		  });
		});
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  _material.SetColor("_EmissionColor", Color.black);
		}
		if (State == 1)
		  GetComponent<Collider>().enabled = false;
	 }
  }
}