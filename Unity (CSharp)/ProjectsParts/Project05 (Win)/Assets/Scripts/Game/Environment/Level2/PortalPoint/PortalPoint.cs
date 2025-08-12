using UnityEngine;
using System.Collections;
using UnityEngine.VFX;
using it.Game.Player;
using DG.Tweening;

namespace it.Game.Environment.Level2
{
  /// <summary>
  /// Точка портала
  /// </summary>
  public class PortalPoint : Environment
  {
	 [SerializeField]
	 private Light _light;
	 [SerializeField]
	 private VisualEffect _effect;
	 [SerializeField]
	 private PortalPoint _targetPortal;

	 private bool DisableTeleport { get; set; }

	 public bool TeleportActive {	get => State == 1;	 }

	 public void Portal()
	 {
		_targetPortal.DisableTeleport = true;
		PlayerBehaviour.Instance.PortalJump(_targetPortal.transform);
	 }

	 protected override void Start()
	 {
		base.Start();
		_effect.SendEvent("OnPlay");
	 }

	 public void Active()
	 {
		if (State == 1) return;

		State = 1;
		ActivePortal();
		Save();
	 }

	 private void ActivePortal()
	 {
		_effect.SendEvent("OnActive");
		_light.DOIntensity(3, 1);
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		if (State == 0)
		{
		  SetDeactivePortal();
		}
		else
		{
		  SetActivePortal();
		  DisableTeleport = false;
		}
	 }

	 [ContextMenu("SetActivePortal")]
	 private void SetActivePortal()
	 {
		_effect.SendEvent("OnActive");
		_light.intensity = 3;
	 }

	 [ContextMenu("SetDeactivePortal")]
	 private void SetDeactivePortal()
	 {
		_effect.SendEvent("OnDeactive");
		_light.intensity = 1;
		Save();
	 }

	 public void RegionEnter()
	 {
		Active();
		if (!TeleportActive)
		  SetActivePortal();
	 }

	 public void RegionExit()
	 {
		DisableTeleport = false;
	 }

	 public void TeleportEnter()
	 {
		if (!_targetPortal.TeleportActive)
		  return;

		if (DisableTeleport)
		  return;

		Portal();
	 }

  }
}