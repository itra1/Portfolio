using UnityEngine;
using System.Collections;
using it.Game.Player.Interactions;
using DG.Tweening;
using it.Game.Items.Inventary;

namespace it.Game.Environment.Level2
{
  /// <summary>
  /// Постамент с грибом
  /// </summary>
  public class MushroomPostament : Environment, IInteractionCondition
  {
	 [SerializeField]
	 private Renderer _shield;
	 [SerializeField]
	 private GoldMushroom _item;

	 public void StartInteraction()
	 {
		if (State == 2)
		  return;

		_item.ColorHide(() =>
		{
		  _item.GetItem();
		});

		State = 2;
		Save();
	 }

	 public void DeactiveShield()
	 {
		if (State > 0)
		  return;

		State = 1;
		Save();
		DOTween.To(() => _shield.material.GetFloat("_Dissolve"), (x) => _shield.material.SetFloat("_Dissolve", x), 0, 1);
	 }

	 public bool InteractionReady()
	 {
		return State == 1;
	 }
  }
}