using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Level1
{
  public class LakeFlyStones : Environment
  {
	 /*
	  * Состояния
	  * 0 - стандартное
	  * 1 - летают
	  */

	 public Transform _lake;
	 public Transform[] _stones;

	 public void Interaction()
	 {
		if (State == 1)
		  return;

		State = 1;
		Save();
		MoveObjects();


	 }

	 protected override void Start()
	 {
		base.Start();
		for (int i = 0; i < _stones.Length; i++)
		{
		  PortalsFX_RandomMoves comp = _stones[i].gameObject.GetComponent<PortalsFX_RandomMoves>();
		  if (comp == null)
			 comp = _stones[i].gameObject.AddComponent<PortalsFX_RandomMoves>();
		  comp.enabled = false;
		}
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{

		  for (int i = 0; i < _stones.Length; i++)
		  {
			 PortalsFX_RandomMoves comp = _stones[i].gameObject.GetComponent<PortalsFX_RandomMoves>();
			 if (comp == null)
				comp = _stones[i].gameObject.AddComponent<PortalsFX_RandomMoves>();
			 comp.enabled = false;
		  }

		  if (State == 0)
		  {
			 _lake.localPosition = Vector3.zero;
			 for (int i = 0; i < _stones.Length; i++)
			 {
				_stones[i].localPosition = Vector3.zero;
				_stones[i].gameObject.GetComponent<PortalsFX_RandomMoves>().enabled = false;
			 }
		  }
		  else
		  {
			 _lake.localPosition = Vector3.up * -0.25f;
			 for (int i = 0; i < _stones.Length; i++)
			 {
				_stones[i].localPosition = Vector3.up * 3f;
				_stones[i].gameObject.GetComponent<PortalsFX_RandomMoves>().enabled = true;
			 }
		  }
		}

	 }

	 private void MoveObjects()
	 {
		_lake.DOLocalMove(Vector3.up * -0.25f, 1);
		for (int i = 0; i < _stones.Length; i++)
		{
		  Transform stone = _stones[i];
		  stone.DOLocalMove(Vector3.up * 3f, 1).OnComplete(() =>
		  {
			 PortalsFX_RandomMoves comp = stone.GetComponent<PortalsFX_RandomMoves>();
			 if (comp != null)
				comp = stone.gameObject.AddComponent<PortalsFX_RandomMoves>();
			 comp.enabled = true;
		  });
		}
	 }


  }
}