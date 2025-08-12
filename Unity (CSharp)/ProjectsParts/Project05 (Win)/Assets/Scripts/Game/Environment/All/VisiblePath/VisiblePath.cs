using UnityEngine;
using System.Collections;

namespace it.Game.Environment.All.VisiblePath
{
  public class VisiblePath : Environment
  {
	 /*
	  * Состояния:
	  * 0 - ожидание
	  * 1 - активирован
	  * 2 - отработало или необходимость отпала
	  * 
	  */

	 [SerializeField]
	 private GameObject _ghost;

	 [SerializeField]
	 private GameObject _path;

	 private float _timeWaint = 60;

	 public void ShowPath()
	 {
		if (State > 0)
		  return;

		State = 1;
		ConfirmState();
		Save();

		InvokeSeconds(()=> { DeactivePath(); }, _timeWaint);
	 }

	 public void DeactivePath(bool force = false)
	 {
		if (State >= 2)
		  return;

		State = 2;
		ConfirmState(force);
		Save();
	 }


	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  _ghost.gameObject.SetActive(State <= 0);
		}
		_path.gameObject.SetActive(State == 1);
	 }

  }
}