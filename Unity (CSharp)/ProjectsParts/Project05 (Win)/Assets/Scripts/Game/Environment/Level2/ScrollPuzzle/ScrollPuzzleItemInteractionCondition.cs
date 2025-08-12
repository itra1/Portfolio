using UnityEngine;
using System.Collections;
namespace it.Game.Environment.Level2
{
  public class ScrollPuzzleItemInteractionCondition : MonoBehaviourBase, it.Game.Player.Interactions.IInteractionCondition
  {
	 private ScrollPuzzle _scrollPuzzle;
	 private ScrollPuzzleItem _item;

	 private void Start()
	 {
		_scrollPuzzle = GetComponentInParent<ScrollPuzzle>();
		_item = GetComponent<ScrollPuzzleItem>();
	 }

	 public bool InteractionReady()
	 {
		return !_item.IsOpen && !string.IsNullOrEmpty(_scrollPuzzle.GetInventaryItem());
	 }
  }
}