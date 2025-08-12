using UnityEngine;
using System.Collections;
using it.Game.Player;
using DG.Tweening;

namespace it.Game.NPC.Enemyes.Boses.SarDayyni
{
  public class SarDayyniTeleport : MonoBehaviour
  {
	 [HideInInspector]
	 public Vector3 target;

	 [SerializeField]
	 private Renderer _blackHole;
	 [SerializeField]
	 private Renderer _distort;
	 [SerializeField]
	 private ParticleSystem _vortex;

	 private bool _isTeleported;
	 private Tween _deactiveTween;

	 private void OnEnable()
	 {
		_isTeleported = false;
		SetActiveGate();
		_deactiveTween = DOVirtual.DelayedCall(2, () => {
		  SetDeactiveGate();
		});
	 }

	 public void PlayerTeleport()
	 {
		if (_isTeleported)
		  return;

		PlayerBehaviour.Instance.PortalJump(target,false);
		gameObject.transform.position = target;

		_deactiveTween.Kill();

		DOVirtual.DelayedCall(1, () => {
		  SetDeactiveGate();
		});
	 }

	 private void SetActiveGate()
	 {
		_blackHole.material.SetFloat("_Power", 0);
		_blackHole.material.DOFloat(2, "_Power", 0.5f);
		_distort.material.SetFloat("_FrenselExp1", 0);
		_distort.material.DOFloat(0.03f, "_FrenselExp1", 0.5f);
		_vortex.Stop();
		DOVirtual.DelayedCall(0.4f,()=>{
		  _vortex.Play();
		});
	 }

	 private void SetDeactiveGate()
	 {
		_blackHole.material.DOFloat(0, "_Power", 0.5f);
		_distort.material.DOFloat(0, "_FrenselExp1", 0.5f);
		_vortex.Stop();
		DOVirtual.DelayedCall(0.5f, () => {
		  gameObject.SetActive(false);
		});
	 }

  }
}