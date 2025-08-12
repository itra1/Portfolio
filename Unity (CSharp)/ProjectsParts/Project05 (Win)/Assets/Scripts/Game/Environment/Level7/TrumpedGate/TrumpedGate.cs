using UnityEngine;
using System.Collections;
using it.Game.Environment.Handlers;
using DG.Tweening;
using Leguar.TotalJSON;

namespace it.Game.Environment.Level7.TrumpedGate
{
  public class TrumpedGate : Environment
  {
	 [SerializeField]
	 private PegasusController _cameraToElevator;
	 [SerializeField]
	 private it.Game.Environment.All.Doors.Door _door;

	 private bool _isHalberd = false;
	 private bool _isStuff = false;

	 public void ActiveHalberd(UnityEngine.Events.UnityAction callback)
	 {
		Use(false, true, callback);
	 }
	 public void ActiveStuff(UnityEngine.Events.UnityAction callback)
	 {
		Use(true, false, callback);
	 }

	 private void Use(bool isStuff, bool isHalberd, UnityEngine.Events.UnityAction callback)
	 {
		_cameraToElevator.Activate(() =>
		{
		  DOVirtual.DelayedCall(1f, () =>
		  {
			 OpenGate(isStuff, isHalberd);
		  });

		  DOVirtual.DelayedCall(5f, () =>
		  {
			 _cameraToElevator.Deactivate();
			 callback?.Invoke();
		  });

		});
	 }

	 private void OpenGate(bool isStuff, bool isHalberd)
	 {
		if (isStuff)
		  _isStuff = isStuff;
		if (isHalberd)
		  _isHalberd = isHalberd;

		if (_isStuff && _isHalberd)
		{
		  _door.Open();
		  State = 2;
		  Save();
		}

	 }

	 protected override JValue SaveData()
	 {
		JSON data = new JSON();
		data.Add("isHalberd", _isHalberd);
		data.Add("isStuff", _isStuff);
		return data;
	 }

	 protected override void LoadData(JValue data)
	 {
		JSON loadData = data as JSON;
		_isHalberd = loadData.GetBool("isHalberd");
		_isStuff = loadData.GetBool("isStuff");
	 }

  }
}