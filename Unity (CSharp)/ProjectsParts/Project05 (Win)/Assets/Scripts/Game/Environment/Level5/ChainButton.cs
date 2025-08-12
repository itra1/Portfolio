using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level5
{
  
  public class ChainButton : Environment
  {
	 [SerializeField]
	 private Transform _chains;

	 private Transform _platform;

	 public void Interaction()
	 {
		State = 2;
		_chains.DOMoveY(-1.7f, 0.5f).OnComplete(() =>
		{

		  _chains.DOMoveY(0, 1.5f);

		});
		Save();
	 }

  }
}