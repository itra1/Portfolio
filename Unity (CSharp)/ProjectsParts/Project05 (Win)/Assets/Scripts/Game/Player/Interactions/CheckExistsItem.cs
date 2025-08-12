using UnityEngine;
using System.Collections;

namespace it.Game.Player.Interactions
{
  public class CheckExistsItem : MonoBehaviour, IInteractionCondition
  {
	 [SerializeField]
	 private string _uuid;
	 public bool InteractionReady()
	 {
		return Managers.GameManager.Instance.Inventary.ExistsItem(_uuid);
	 }
  }
}