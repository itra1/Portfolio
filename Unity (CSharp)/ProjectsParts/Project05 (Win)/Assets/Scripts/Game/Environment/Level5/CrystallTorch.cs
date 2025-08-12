using UnityEngine;
using System.Collections;
using it.Game.Player;
using DG.Tweening;
using Leguar.TotalJSON;
using UnityEditor;

namespace it.Game.Environment.Level5
{
  public class CrystallTorch : Environment
  {
	 public it.Game.Items.CrystalTorch _torch;
	 private bool _isActiveIntencity;

	 protected override void Awake()
	 {
		base.Awake();
		_torch._manager = this;
		ActiveIntencity();
		_torch.onHold = () =>
		{
		  ItemHold();
		};
	 }

	 public void HideIntensity()
	 {
		_isActiveIntencity = false;

		_torch.Light.DOIntensity(1f, 0.5f);
		Save();
	 }

	 public void ActiveIntencity()
	 {
		_isActiveIntencity = true;
		_torch.Light.DOIntensity(2.5f, 0.5f);
		Save();
	 }

	 private void ItemHold()
	 {
		State = 1;
		Save();
	 }

	 [ContextMenu("Drop item")]
	 public void DropItem()
	 {
		if (State != 1)
		  return;
		State = 2;
		Save();
		PlayerBehaviour.Instance.MotionController.GetMotion<it.Game.Player.MotionControllers.Motions.HoldItem>().DropItem();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		if (isForce)
		{
		  if (State == 1)
		  {
			 StartCoroutine(SetItem());
			 //PlayerBehaviour.Instance.MotionController.GetMotion<it.Game.Player.MotionControllers.Motions.HoldItem>().SetCustomHold(_torch);
		  }
		  if (State > 1)
		  {
			 _torch.gameObject.SetActive(false);
		  }
		  if (State == 0)
		  {
			 _torch.gameObject.SetActive(true);
		  }
		  if (_isActiveIntencity)
			 ActiveIntencity();
		  else
			 HideIntensity();
		}
	 }

	 private IEnumerator SetItem()
	 {
		bool isSet = false;
		while (!isSet)
		{
		  if (PlayerBehaviour.Instance != null)
		  {
			 PlayerBehaviour.Instance.MotionController.GetMotion<it.Game.Player.MotionControllers.Motions.HoldItem>().SetItem(_torch);
			 isSet = true;
		  }
		  yield return null;
		}

	 }

	 protected override JValue SaveData()
	 {
		JSON saveData = new JSON();
		saveData.Add("incityLight", _isActiveIntencity);
		return saveData;
	 }

	 protected override void LoadData(JValue data)
	 {
		base.LoadData(data);

		JSON loadData = data as JSON;
		_isActiveIntencity = loadData.GetBool("incityLight");
	 }

  }
}