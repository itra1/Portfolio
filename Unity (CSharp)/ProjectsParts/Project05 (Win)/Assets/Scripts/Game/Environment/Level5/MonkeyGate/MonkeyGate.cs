using UnityEngine;
using System.Collections;
using Leguar.TotalJSON;
using it.Game.Handles;
using DG.Tweening;

namespace it.Game.Environment.Level5.MonkeyGates
{
  public class MonkeyGate : Environment
  {
	 public Renderer _field;
	 public GameObject[] _roundCrystal;
	 public MonkeyStella[] _monkeys;
	 protected override void Awake()
	 {
		base.Awake();
		_field.material = Instantiate(_field.material);

		for (int i = 0; i < _monkeys.Length; i++)
		{
		  _monkeys[i].OnActivate = ItemComplete;
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

		_field.material.DOFloat(0, "_Dissolve", 1).OnComplete(()=> {
		  _field.gameObject.SetActive(false);
		});

		for (int i = 0; i < _roundCrystal.Length; i++)
		{
		  _roundCrystal[i].GetComponent<RotateAround>().IsActived = false;
		}
		Save();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		if (isForce)
		{
		  _field.gameObject.SetActive(State != 2);
		  _field.material.SetFloat("_Dissolve", 1);

		  for (int i = 0; i < _roundCrystal.Length; i++)
		  {
			 _roundCrystal[i].GetComponent<RotateAround>().IsActived = State != 2;
		  }

		}
	 }

	 protected override void BeforeLoad()
	 {
		for (int i = 0; i < _monkeys.Length; i++)
		{
		  _monkeys[i].Deactive();
		}
	 }

	 private void CheckFullComplete()
	 {
		bool isFull = true;
		for (int i = 0; i < _monkeys.Length; i++)
		{
		  if (!_monkeys[i].IsActive)
			 isFull = false;
		}

		if (isFull)
		  FullComplete();
	 }

	 protected override void LoadData(JValue data)
	 {
		base.LoadData(data);
		JSON items = data as JSON;
		for (int i = 0; i < _monkeys.Length; i++)
		{
		  bool isActive = items.ContainsKey(_monkeys[i].Uuid) ? items.GetBool(_monkeys[i].Uuid) : false;

		  if (isActive)
			 _monkeys[i].Activate();
		  else
			 _monkeys[i].Deactive();

		}
	 }

	 protected override JValue SaveData()
	 {
		JSON items = new JSON();

		for (int i = 0; i < _monkeys.Length; i++)
		{
		  items.Add(_monkeys[i].Uuid, _monkeys[i].IsActive);
		}

		return items;
	 }
  }
}