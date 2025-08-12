using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Environment.All.Ladder
{
  public class StoneLadder : Ladder
  {
	 [SerializeField]
	 private RangeFloat _itemDistance;
	 [SerializeField]
	 private int _tryIterationCount = 5;
	 [ContextMenu("Spawn")]
	 protected override void Spawn()
	 {
		base.Spawn();


		if (_itemDistance.Max <= 0)
		{
		  Debug.Log("Не корректная дистанция между предметами");
		  return;
		}

		RecursiveSpawn(new Vector3(0, _itemDistance.RandomRange));

		//_correntItem = -1;

		//while (localPosition < _height * _cut)
		//{
		//  Transform item = InstanceItem();

		//  item.localPosition = new Vector3(0, localPosition);
		//  localPosition += _itemDistance;
		//}

	 }



	 private void RecursiveSpawn(Vector3 localPosition)
	 {
		Transform item = InstanceItem();
		item.localPosition = localPosition;
		item.rotation = Quaternion.Euler(Random.Range(-180,180), Random.Range(-180, 180), Random.Range(-180, 180));

		int readyIterations = _tryIterationCount;

		while(readyIterations > 0)
		{
		  readyIterations--;

		  Vector3 randomVector = (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)).normalized;

		  Vector3 newPosition = localPosition + randomVector * _itemDistance.RandomRange;

		  if (Mathf.Abs(newPosition.x) > (_width / 2) - 0.1f)
			 continue;

		  if (newPosition.y < 0 || newPosition.y > (_height*_cut)-0.1f)
			 continue;
		  if (!CheckOtherItemDistance(newPosition))
			 continue;

		  RecursiveSpawn(newPosition);

		}


	 }

	 private bool CheckOtherItemDistance(Vector3 position)
	 {
		foreach(var elem in _itemsList)
		{
		  if ((elem.transform.localPosition - position).magnitude < _itemDistance.Min)
			 return false;
		}
		return true;
	 }


  }
}