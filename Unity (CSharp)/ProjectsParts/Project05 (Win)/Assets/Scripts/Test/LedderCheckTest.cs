using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Geometry;


public class LedderCheckTest : MonoBehaviourBase
{
  [SerializeField]
  private it.Game.Environment.All.Ladder.Ladder _ladder;

  [SerializeField]
  private LayerMask _mask;

  // Update is called once per frame
  void Update()
  {
	 if (_ladder == null || _ladder.ItemsList.Count == 0)
		return;

	 RaycastHit rayInfo;
	 if (RaycastExt.SafeRaycast(transform.position, transform.forward, out rayInfo, 2f, _mask, transform))
	 {
		float diff = (new Vector3(rayInfo.point.x, 0, rayInfo.point.z) - new Vector3(_ladder.transform.position.x, 0, _ladder.transform.position.z)).magnitude;
		GameObject target = GetNearest();

		Vector3 targetPosition = target.transform.position + (new Vector3(rayInfo.point.x, 0, rayInfo.point.z) - new Vector3(_ladder.transform.position.x, 0, _ladder.transform.position.z));

		//Vector3 c = new Vector3(targetPosition.x, 0, targetPosition.z) - new Vector3(transform.position.x, 0, transform.position.z);

		com.ootii.Graphics.GraphicsManager.DrawLine(transform.position, targetPosition, Color.red, null, 0.1f);
		com.ootii.Graphics.GraphicsManager.DrawLine(new Vector3(_ladder.transform.position.x, 0, _ladder.transform.position.z), new Vector3(rayInfo.point.x, 0, rayInfo.point.z), Color.red, null, 0.1f);
		//com.ootii.Graphics.GraphicsManager.DrawLine(transform.position, transform.forward, Color.red, null, 0.1f);
	 }


  }

  private GameObject GetNearest()
  {
	 float maxDistance = 999999;
	 GameObject nearest = null;
	 for (int i = 0; i < _ladder.ItemsList.Count; i++)
	 {
		if ((_ladder.ItemsList[i].transform.position - transform.position).sqrMagnitude < maxDistance)
		{
		  maxDistance = (_ladder.ItemsList[i].transform.position - transform.position).sqrMagnitude;
		  nearest = _ladder.ItemsList[i];
		}
	 }
	 return nearest;
  }
}
