using System.Collections;
using System.Collections.Generic;
using com.ootii.Messages;
using it.Game.Player.Save;
using UnityEngine;
using Leguar.TotalJSON;

namespace it.Game.Environment.Level1.StoneBridge
{

  public class StoneBridge : Environment
  {
	 /*
	  * Статусы:
	  * 0 - стандарт
	  * 1 - готовность разрушиться
	  * 2 - разрушено
	  * 
	  */

	 [SerializeField]
	 private Section[] _sections;

	 [System.Serializable]
	 private struct Section
	 {
		public ObjectData[] objectData;
	 }

	 //static JValue defaultData = null;

	 [SerializeField]
	 private Game.Handles.TriggerHandler _triggerHandle;

	 protected override void Awake()
	 {
		base.Awake();

		//if (defaultData == null)
		//  defaultData = SaveData();
	 }

	 protected override void Start()
	 {
		base.Start();

		if (State == 0)
		  SetStartPosition();

		_triggerHandle.onTriggerEnter.AddListener((collider) =>
		{
		  var player = collider.GetComponent<Game.Player.PlayerBehaviour>();

		  if (player == null)
			 return;

		  if (State == 1)
		  {
			 DestroyBridge();
			 State = 2;
			 Save();
		  }
		});
	 }

	 [ContextMenu("SetStartPosition")]
	 public void SetStartPosition()
	 {

		for (int i = 0; i < _sections.Length; i++)
		{
		  for (int x = 0; x < _sections[i].objectData.Length; x++)
		  {
			 _sections[i].objectData[x].obj.position = _sections[i].objectData[x]._position;
			 _sections[i].objectData[x].obj.localEulerAngles = _sections[i].objectData[x]._rotateion;
			 Rigidbody rb = _sections[i].objectData[x].obj.GetComponent<Rigidbody>();
			 if(rb != null)
			 {
				rb.isKinematic = true;
				rb.useGravity = false;
			 }
		  }
		}
	 }

	 [ContextMenu("ReadData")]
	 public void ReadData()
	 {
		for (int i = 0; i < _sections.Length; i++)
		{
		  for (int x = 0; x < _sections[i].objectData.Length; x++)
		  {
			 _sections[i].objectData[x]._position = _sections[i].objectData[x].obj.position;
			 _sections[i].objectData[x]._rotateion = _sections[i].objectData[x].obj.localEulerAngles;
		  }
		}
	 }

	 protected override void OnDestroy()
	 {
		base.OnDestroy();
	 }

	 [ContextMenu("Destroy Ready")]
	 public void DestroyReady()
	 {
		State = 1;
		Save();
	 }

	 [ContextMenu("Destroy bridge")]
	 public void DestroyBridge()
	 {
		if (State != 1)
		  return;
		if (!Application.isPlaying)
		  return;
		StartCoroutine(DestroyCor());

	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce && State == 0)
		{
		  //LoadData(Newtonsoft.Json.JsonConvert.SerializeObject(defaultData));
		  SetStartPosition();
		  //LoadData(defaultData);
		}

	 }

	 private IEnumerator DestroyCor()
	 {
		yield return new WaitForSeconds(5);
		foreach (var sect in _sections)
		{
		  foreach (var elem in sect.objectData)
		  {
			 Rigidbody rb = elem.obj.gameObject.GetComponent<Rigidbody>();
			 rb.isKinematic = false;
			 rb.useGravity = true;
			 if (elem.setForce)
			 {

				Vector3 force = (Random.value > 0.5f ? Vector3.left : Vector3.right) * 50;
				rb.AddForceAtPosition(force, transform.position + Vector3.up * 5, ForceMode.Acceleration);
				rb.AddForceAtPosition(force, transform.position + Vector3.up * 5, ForceMode.Acceleration);
			 }
		  }

		  //foreach (var elem in sect.objects)
		  //{
		  //Rigidbody rb = elem.gameObject.GetComponent<Rigidbody>();
		  //rb.isKinematic = false;
		  //rb.useGravity = true;
		  //}

		  //foreach (var colum in sect.colums)
		  //{
		  //Rigidbody rb = colum.GetComponent<Rigidbody>();
		  //Vector3 force = (Random.value > 0.5f ? Vector3.left : Vector3.right) * 50;
		  //rb.AddForceAtPosition(force, transform.position + Vector3.up * 5, ForceMode.Acceleration);
		  //rb.AddForceAtPosition(force, transform.position + Vector3.up * 7, ForceMode.Acceleration);
		  //}
		  yield return new WaitForSeconds(5);
		}
		yield return new WaitForSeconds(5);
		Save();
	 }

	 [System.Serializable]
	 public struct ObjectData
	 {
		public Transform obj;
		public Vector3 _position;
		public Vector3 _rotateion;
		public bool setForce;
	 }

	 #region Save

	 protected override void LoadData(Leguar.TotalJSON.JValue data)
	 {
		StopAllCoroutines();
		base.LoadData(data);

		JSON[] saveItems = (data as JArray).AsJSONArray();

		int index = 0;

		foreach (var sect in _sections)
		{
		  foreach (var elemObj in sect.objectData)
		  {
			 if (elemObj.obj.GetComponent<Rigidbody>() != null)
				DestroyImmediate(elemObj.obj.GetComponent<Rigidbody>());

			 elemObj.obj.position = new Vector3(saveItems[index].GetJSON("pos").GetFloat("x"),
				saveItems[index].GetJSON("pos").GetFloat("y"),
				saveItems[index].GetJSON("pos").GetFloat("z"));
			 elemObj.obj.localEulerAngles = new Vector3(saveItems[index].GetJSON("rot").GetFloat("x"),
				saveItems[index].GetJSON("rot").GetFloat("y"),
				saveItems[index].GetJSON("rot").GetFloat("z"));

			 index++;
		  }
		}
	 }

	 protected override JValue SaveData()
	 {
		JArray daveItem = new JArray();
		foreach (var sect in _sections)
		{
		  foreach (var elem in sect.objectData)
		  {
			 JSON element = new JSON();

			 JSON pos = new JSON();
			 pos.Add("x", elem.obj.position.x);
			 pos.Add("y", elem.obj.position.y);
			 pos.Add("z", elem.obj.position.z);
			 element.Add("pos", pos);

			 JSON rot = new JSON();
			 rot.Add("x", elem.obj.localEulerAngles.x);
			 rot.Add("y", elem.obj.localEulerAngles.y);
			 rot.Add("z", elem.obj.localEulerAngles.z);
			 element.Add("rot", rot);

			 daveItem.Add(element);
		  }

		}
		//return Newtonsoft.Json.JsonConvert.SerializeObject(daveItem);

		return daveItem;
	 }

	 #endregion
  }
}