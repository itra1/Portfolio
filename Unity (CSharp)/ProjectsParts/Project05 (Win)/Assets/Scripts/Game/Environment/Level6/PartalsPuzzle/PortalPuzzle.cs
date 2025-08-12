using UnityEngine;
using System.Collections;
using Leguar.TotalJSON;

namespace it.Game.Environment.Level6.PortalsPuzzle
{
  public class PortalPuzzle : Environment
  {
	 [SerializeField]
	 private All.HolographicTechnoButton _button1;
	 [SerializeField]
	 private All.HolographicTechnoButton _button2;
	 [SerializeField]
	 private GameObject _platform1;
	 [SerializeField]
	 private GameObject _platform2;

	 [SerializeField]
	 private bool _isButton1 = false;
	 [SerializeField]
	 private bool _isButton2 = false;

	 [ContextMenu("OnButton1")]
	 public void OnButton1()
	 {
		_isButton1 = true;
		ConfirmPlatforms();
		Save();
	 }

	 [ContextMenu("OnButton2")]
	 public void OnButton2()
	 {
		_isButton2 = true;
		ConfirmPlatforms();
		Save();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		ConfirmPlatforms();
	 }

	 private void ConfirmPlatforms()
	 {

		_platform1.SetActive(_isButton1);
		_platform2.SetActive(_isButton2);
	 }

	 protected override JValue SaveData()
	 {
		JSON _data = new JSON();
		_data.Add("1", _isButton1);
		_data.Add("2", _isButton2);
		return _data;
	 }

	 protected override void LoadData(JValue data)
	 {
		JSON _data = data as JSON;
		_isButton1 = _data.GetBool("1");
		_isButton2 = _data.GetBool("2");

	 }

  }
}