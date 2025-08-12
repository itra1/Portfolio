using UnityEngine;
using System.Collections;
using DG.Tweening;
using it.Game.Player.Interactions;
using it.Game.Managers;
using Leguar.TotalJSON;

namespace it.Game.Environment.Level6.BaseElevator
{
  public class BaseElevator : Environment
  {
	 [SerializeField]
	 private Obelisk[] _obelists;
	 [SerializeField]
	 private Transform _platform;
	 [SerializeField]
	 private Transform _triggerTeleport;
	 [SerializeField]
	 private Color _emissionColor = Color.white;

	 protected override void Awake()
	 {
		base.Awake();

		for (int i = 0; i < _obelists.Length; i++)
		{
		  _obelists[i].OnActivate = ItemComplete;
		}

	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (State == 2)
		  SetActivePlatform();
		else
		  SetDeactivePlatform();
	 }
	 private void ItemComplete()
	 {
		Save();
		CheckFullComplete();
	 }

	 private void CheckFullComplete()
	 {
		bool isFull = true;
		for (int i = 0; i < _obelists.Length; i++)
		{
		  if (!_obelists[i].IsActive)
			 isFull = false;
		}

		if (isFull)
		  FullComplete();
	 }

	 private void FullComplete()
	 {
		State = 2;
		SetActivePlatform();
		Save();
	 }

	 private void SetActivePlatform()
	 {
		_triggerTeleport.gameObject.SetActive(true);
		Material mat = _platform.GetComponent<Renderer>().material;
		mat.DOColor(_emissionColor, "_EmissionColor", 1);
	 }
	 private void SetDeactivePlatform()
	 {
		_triggerTeleport.gameObject.SetActive(false);
		Material mat = _platform.GetComponent<Renderer>().material;
		mat.DOColor(Color.black, "_EmissionColor", 1);
	 }
	 public void Portain()
	 {
		if (State != 2)
		  return;
		State = 3;
		it.Game.Managers.GameManager.Instance.NextLevel();
	 }

	 protected override void LoadData(JValue data)
	 {
		base.LoadData(data);
		JSON items = data as JSON;
		for (int i = 0; i < _obelists.Length; i++)
		{
		  bool isActive = items.ContainsKey(_obelists[i].Uuid) ? items.GetBool(_obelists[i].Uuid) : false;

		  if (isActive)
			 _obelists[i].Activate();
		  else
			 _obelists[i].Deactive();

		}
	 }

	 protected override JValue SaveData()
	 {
		JSON items = new JSON();

		for (int i = 0; i < _obelists.Length; i++)
		{
		  items.Add(_obelists[i].Uuid, _obelists[i].IsActive);
		}

		return items;
	 }


  }
}