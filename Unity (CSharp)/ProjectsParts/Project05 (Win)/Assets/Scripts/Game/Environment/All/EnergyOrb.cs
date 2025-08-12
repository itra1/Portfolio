using UnityEngine;
using it.Game.Managers;
using it.Game.Player;
using QFX.IFX;
using DG.Tweening;

namespace it.Game.Environment
{
  public class EnergyOrb : Environment
  {
	 [SerializeField]
	 private GameObject _object;
	 [SerializeField]
	 private QFX.IFX.IFX_HomingParticleSystem _souls;

	 public void Interaction()
	 {
		if (State == 1)
		  return;
		_object.SetActive(false);
		GameManager.Instance.EnergyManager.Add(20);
		ActivateSouls();
		State = 1;
		Save();
	 }


	 private void ActivateSouls()
	 {
		_souls.Target = PlayerBehaviour.Instance.HipBone;
		var an = _souls.GetComponent<IFX_ShaderAnimator>();
		an.Target = PlayerBehaviour.Instance.HipBone;
		_souls.gameObject.SetActive(true);
		_souls.Run();
		DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransformAndForget("Effects", transform, 1, null, 0, "GetOrb");

		DOVirtual.DelayedCall(10, () =>
		{
		  _souls.gameObject.SetActive(false);
		});
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		_souls.gameObject.SetActive(false);
		if (isForce)
		  _object.SetActive(State == 0);

	 }
  }
}