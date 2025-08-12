using UnityEngine;
using System.Collections;
using DG.Tweening;
using it.Game.Player.Interactions;
using it.Game.Managers;

namespace it.Game.Environment.Level5.MonkeyGates
{
  public class MonkeyStella : UUIDBase, IInteractionCondition
  {
	 public UnityEngine.Events.UnityAction OnActivate;
	 [SerializeField]
	 private GameObject _crystal;

	 private bool _isActive;
	 public bool IsActive { get => _isActive; set => _isActive = value; }

	 [SerializeField]
	 private string crystalUuid;

	 public void Interaction()
	 {
		GameManager.Instance.Inventary.Remove(crystalUuid);
		Activate();
		OnActivate?.Invoke();
	 }
	 public void Activate()
	 {
		_isActive = true;
		_crystal.SetActive(true);
	 }

	 public void Deactive()
	 {
		_isActive = false;
		_crystal.SetActive(false);
	 }

	 public bool InteractionReady()
	 {
		if (IsActive)
		  return false;

		return GameManager.Instance.Inventary.ExistsItem(crystalUuid);
	 }
  }
}