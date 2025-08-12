using UnityEngine;
using System.Collections;

namespace it.Game.NPC.Animals
{
  public class Fangtooth : Fish, Game.Items.IUsableParent, Game.Items.IHoldParent
  {

	 [SerializeField]
	 private GameObject _Light;

	 //[SerializeField]
	 //private com.ootii.Demos.MoveToDriverWaypoints _moveWaypoints;
	 [SerializeField]
	 private com.ootii.Actors.MovePlayerOut _moveOut;
	 [SerializeField]
	 private com.ootii.Actors.MoveToDriver _moveDriver;

	 public bool IsUseReady
	 {
		get => true;
	 }


	 protected override void Awake()
	 {
		base.Awake();

		Game.Handles.CheckToPlayerDistance cpd = GetComponent<Game.Handles.CheckToPlayerDistance>();

		if (cpd != null)
		{
		  _Light.SetActive(false);

		  cpd.onPlayerInDistance = () =>
		  {
			 _Light.SetActive(true);
		  };

		  cpd.onPlayerOutDistance = () =>
		  {
			 _Light.SetActive(false);
		  };

		}
		
		//_moveWaypoints.enabled = false;
		_moveOut.enabled = false;
		_moveDriver.enabled = false;
	 }

	 protected override void OnEnable()
	 {
		base.OnEnable();
		Phase = NpcPhase.idle;
		_moveDriver.enabled = true;
		//_moveWaypoints.enabled = true;
	 }

	 /// <summary>
	 /// Старт удержания
	 /// </summary>
	 public void StartHold()
	 {
		SetHoldComponent(false);
		Phase = NpcPhase.hold;
		//_moveWaypoints.enabled = false;
		_moveDriver.enabled = false;
	 }

	 /// <summary>
	 /// Стор удержания
	 /// </summary>
	 public void StopHold()
	 {
		SetHoldComponent(true);
	 }

	 private void SetHoldComponent(bool isEnable)
	 {
		GetComponent<CapsuleCollider>().enabled = isEnable;
		GetComponent<com.ootii.Actors.ActorController>().enabled = isEnable;

	 }

	 public void BeforeStartHold()
	 {
		StartHold();
		return;
	 }

	 public void AfterStartHold()
	 {
		return;
	 }

	 public void BeforeEndHold()
	 {
		return;
	 }

	 public void AfterEndHold()
	 {
		StopHold();
		Phase = NpcPhase.free;
		//_moveWaypoints.enabled = false;
		_moveOut.enabled = true;
		_moveDriver.enabled = true;
		InvokeSeconds(() =>
		{
		  gameObject.SetActive(false);

		}, 5f);
		return;
	 }



  }

}