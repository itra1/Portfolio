using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;
using it.Game.Items;
using it.Game.Items.Inventary;
using DG.Tweening;

namespace it.Game.Environment.Level3.PuzzleGate
{
  public class PuzzleGate : Environment
  {
	 /*
	  * Статусы
	  * 0 - закрытый
	  * 1 - открытый
	  */

	 [SerializeField]
	 private Animator[] _chains;

	 [SerializeField]
	 private string[] _keyUuid;

	 [SerializeField]
	 private KeyData[] _keys = new KeyData[2];

	 public void Open()
	 {
		if (State == 1)
		  return;

		State = 1;
		SetData();
		Save();

	 }

	 private void SetData(bool force = false)
	 {
		if (!force)
		{
		  for (int i = 0; i < _keys.Length; i++)
		  {
			 GameObject inst = _keys[i].Instance;
			 inst.GetComponent<Item>().ColorHide();
			 DOVirtual.DelayedCall(2, () =>
			 {
				DestroyImmediate(inst);
			 });
		  }
		}
		else
		{
		  for (int i = 0; i < _keys.Length; i++)
		  {
			 DestroyImmediate(_keys[i].Instance);
		  }
		}
		  GetComponent<Animator>().SetTrigger(force ? "ForceOpen" : "Open");
		GetComponent<BoxCollider>().enabled = false;
		if (!force)
		{
		  foreach (var elem in _chains)
		  {
			 elem.SetTrigger("play");
		  }
		}
	 }

	 public bool InteractionReady()
	 {
		return !string.IsNullOrEmpty(GetExistsUuid());
	 }

	 private string GetExistsUuid()
	 {
		for (int i = 0; i < _keyUuid.Length; i++)
		{
		  if (it.Game.Managers.GameManager.Instance.Inventary.ExistsItem(_keyUuid[i]))
			 return _keyUuid[i];
		}
		return null;
	 }

	 public void AddKey(int key)
	 {
		string uuid = GetExistsUuid();

		if (string.IsNullOrEmpty(uuid))
		  return;
		_keys[key].uuid = uuid;
		it.Game.Managers.GameManager.Instance.Inventary.Remove(uuid);
		CreateInstance();
		Save();

		OpenIsReady();
	 }

	 private void CreateInstance()
	 {
		for (int i = 0; i < _keys.Length; i++)
		{
		  if(!string.IsNullOrEmpty(_keys[i].uuid) && _keys[i].Instance == null)
		  {

			 GameObject prefab = it.Game.Managers.GameManager.Instance.Inventary.GetPrefab(_keys[i].uuid);

			 GameObject keyInst = Instantiate(prefab, _keys[i].keyPosition);
			 keyInst.transform.localScale = Vector3.one;
			 keyInst.transform.localPosition = Vector3.zero;
			 keyInst.transform.rotation = _keys[i].keyPosition.rotation;

			 InventaryItem ii = keyInst.GetComponent<InventaryItem>();
			 ii.CheckExistsAndDeactive = false;
			 ii.ColorShow(null);
			 _keys[i].Instance = keyInst;
		  }
		}
	 }

	 private void OpenIsReady()
	 {
		for (int i = 0; i < _keys.Length; i++)
		  if (string.IsNullOrEmpty(_keys[i].uuid))
			 return;

		Open();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce && State == 1)
		{
		  SetData(true);
		}
		if(State == 0)
		{
		  ClearInstances();
		  CreateInstance();
		}
	 }

	 private void ClearInstances()
	 {
		for(int i = 0; i < _keys.Length; i++)
		{
		  if(_keys[i].uuid == null && _keys[i].Instance != null)
			 DestroyImmediate(_keys[i].Instance);

		  //_keys[i].uuid = null;
		}
	 }

	 #region Save

	 protected override void LoadData(Leguar.TotalJSON.JValue data)
	 {
		StopAllCoroutines();
		base.LoadData(data);

		for (int i = 0; i < _keys.Length; i++)
		{
		  _keys[i].uuid = null;
		}

		JSON[] saveData = (data as JArray).AsJSONArray();

		for (int i = 0; i < saveData.Length; i++)
		{
		  _keys[i].uuid = saveData[i].GetString("uuid");
		}

	 }

	 protected override JValue SaveData()
	 {
		JArray saveData = new JArray();
		foreach (var key in _keys)
		{
		  JSON elem = new JSON();
		  elem.Add("uuid", key.uuid);

		  saveData.Add(elem);

		}

		return saveData;
	 }

	 #endregion


	 [System.Serializable]
	 private struct KeyData
	 {
		public string uuid;
		public Transform keyPosition;
		public GameObject InteractionBlock;
		[HideInInspector]
		public GameObject Instance;
	 }

  }
}