using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Environment.All.Ladder
{
  public class StandartLadder : Ladder
  {
	 [SerializeField]
	 private float _itemDistance = .3f;

	 [ContextMenu("Spawn")]
	 protected override void Spawn()
	 {
		base.Spawn();

		if (_itemDistance <= 0)
		{
		  Debug.Log("Не корректная дистанция между предметами");
		  return;
		}

		float localPosition = 0 + _itemDistance;

		_correntItem = -1;

		while (localPosition < _height * _cut)
		{
		  Transform item = InstanceItem();

		  item.localPosition = new Vector3(0, localPosition);
		  localPosition += _itemDistance;
		}

	 }
  }
}