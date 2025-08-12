using UnityEngine;
using HutongGames.PlayMaker;
using Pegasus;

namespace it.Game.PlayMaker
{
  [ActionCategory("Pegasus")]
  public class PegasusCheckSegment : PegasusBase
  {
	 [RequiredField]
	 public FsmGameObject poiObject;
	 public FsmEvent onSegment;

	 private PegasusPoi _poi;

	 public override void OnEnter()
	 {
		base.OnEnter();

		_poi = poiObject.Value.GetComponent<PegasusPoi>();
	 }

	 public override void OnUpdate()
	 {
		if (_pegasus.PegasusManager.m_currentSegment.Equals(_poi))
		{
		  Fsm.Event(onSegment);
		}
	 }

  }
}