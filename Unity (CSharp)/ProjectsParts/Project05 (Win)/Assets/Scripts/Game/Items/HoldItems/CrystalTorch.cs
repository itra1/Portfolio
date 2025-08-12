using UnityEngine;
using System.Collections;
using UnityEditor;
using it.Game.Player.Interactions;

namespace it.Game.Items
{
  public class CrystalTorch : HoldenItem, IInteractionCondition
  {
	 public UnityEngine.Events.UnityAction onHold;

	 [SerializeField]
	 private Light _light;

	 public Light Light { get => _light; set => _light = value; }

	 [HideInInspector]
	 public it.Game.Environment.Level5.CrystallTorch _manager;

	 public override void Hold()
	 {
		base.Hold();
		onHold?.Invoke();
	 }

	 public bool InteractionReady()
	 {
		return true;
	 }

	 private void Update()
	 {

		if (!_isHolded)
		{
		  if (transform.position.y < 140)
			 transform.position = Player.PlayerBehaviour.Instance.transform.position;
		}

	 }

  }
}