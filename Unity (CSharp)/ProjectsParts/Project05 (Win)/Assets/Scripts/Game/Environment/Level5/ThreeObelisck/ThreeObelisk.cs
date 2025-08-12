using UnityEngine;
using System.Collections;
using Leguar.TotalJSON;
using MirzaBeig.ParticleSystems.Demos;
using it.Game.Handles;

namespace it.Game.Environment.Level5.ThreeObelisks
{
  /// <summary>
  /// Основной менеджер квеста
  /// </summary>
  public class ThreeObelisk : Environment
  {
	 public GameObject _field;
	 public GameObject[] _roundCrystal;
	 public Obelisk[] obelisks;

	 protected override void Awake()
	 {
		base.Awake();

		for(int i = 0; i < obelisks.Length; i++)
		{
		  obelisks[i].OnActivate = ItemComplete;
		}

	 }

	 private void ItemComplete()
	 {
		Save();
		CheckFullComplete();
	 }

	 private void FullComplete()
	 {
		State = 2;
		_field.SetActive(false);
		for (int i = 0; i < _roundCrystal.Length; i++)
		{
		  _roundCrystal[i].GetComponent<RotateAround>().IsActived = false;
		}
		Save();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		CheckFullComplete();
	 }

	 private void CheckFullComplete()
	 {
		bool isFull = true;
		for (int i = 0; i < obelisks.Length; i++)
		{
		  if (!obelisks[i].IsActive)
			 isFull = false;
		}

		if (isFull)
		  FullComplete();
	 }

	 protected override void LoadData(JValue data)
	 {
		base.LoadData(data);
		JSON items = data as JSON;
		for (int i = 0; i < obelisks.Length; i++)
		{
		  bool isActive = items.ContainsKey(obelisks[i].Uuid) ? items.GetBool(obelisks[i].Uuid) : false;

		  if (isActive)
			 obelisks[i].Activate();
		  else
			 obelisks[i].Deactive();

		}
	 }

	 protected override JValue SaveData()
	 {
		JSON items = new JSON();

		for (int i = 0; i < obelisks.Length; i++)
		{
		  items.Add(obelisks[i].Uuid, obelisks[i].IsActive);
		}

		return items;
	 }


  }
}