using System.Collections.Generic;
using it.Game.Handles;
using UnityEngine;
using UnityEngine.Events;
using it.Game.Items;
namespace it.Game.Environment.Challenges.Level7.Terminal
{
  public class TerminalItem : MonoBehaviourBase
  {
	 [SerializeField]
	 private int _index;
	 public int Index => _index;
	 [SerializeField]
	 private Collider _collider;
	 [SerializeField]
	 private TextMesh _value;

	 public void SetReady(bool isReady)
	 {
		_collider.enabled = isReady;
	 }

	 public void SetValue(int value)
	 {
		_value.text = value.ToString();
	 }
  }
}