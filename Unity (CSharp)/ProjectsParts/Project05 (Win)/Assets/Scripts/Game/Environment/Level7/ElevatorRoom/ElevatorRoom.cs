using UnityEngine;
using it.Game.Environment.Handlers;
using DG.Tweening;
using it.Game.Items.Inventary;

#if UNITY_EDITOR

#endif

namespace it.Game.Environment.Level7.ElevatorRoom
{
  /// <summary>
  /// Елеватор
  /// </summary>
  public class ElevatorRoom : Environment
  {
	 /*
	  * Статусы:
	  * 
	  * 0 - отключен
	  * 1 - включен
	  * 
	  */

	 [SerializeField]
	 private MovePlatform _platform;
	 [SerializeField]
	 private Animator _animatorGlassGate;
	 [SerializeField]
	 private PegasusController _cameraToElevator;
	 [SerializeField]
	 private Halberd _halbers;

	 //public void SetHelbard()
	 //{
		//_halbers.CheckExistsAndDeactive = false;
		//_halbers.ColorShow(()=> {
		//  ActiveHalberd();
		//});
	 //}

	 public void ActiveHalberd(UnityEngine.Events.UnityAction callback)
	 {
		_cameraToElevator.Activate(() =>
		{
		  DOVirtual.DelayedCall(1f, () =>
		  {

			 _animatorGlassGate.SetBool("Open", true);
			 _platform.Activate();
		  });

		  DOVirtual.DelayedCall(5f, () =>
		  {
			 _cameraToElevator.Deactivate();
			 //ResumeAction();
			 callback?.Invoke();
		  });

		});
	 }

	 //public void ResumeAction()
	 //{
		//_halbers.ColorHide(() => {
		//  PlayerBehaviour.Instance.InteractionSystem.ResumeAll();
		//});
	 //}
  }
}