using UnityEngine;
using System.Collections;
using it.Game.Player;
using Leguar.TotalJSON;
using DG.Tweening;

namespace it.Game.Environment.Level3
{
  public class SkullTemplate : Environment
  {
	 [SerializeField]
	 private FireBowl[] _fireBowl;

	 [SerializeField]
	 private Material _groundMaterial;
	 [SerializeField]
	 [ColorUsage(true,true)]
	 private Color _emis1;
	 [SerializeField]
	 [ColorUsage(true, true)]
	 private Color _emis2;
	 [SerializeField]
	 private Light[] _innerRound;
	 [SerializeField]
	 private Light[] _outerRound;
	 [SerializeField]
	 private Light[] _stoneEyes;

	 [SerializeField]
	 private Light _doorLight;
	 [SerializeField]
	 private Transform _doorLeft;
	 [SerializeField]
	 private Transform _doorRight;

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  if(State == 0)
		  {
			 for (int i = 0; i < _innerRound.Length; i++)
			 {
				_innerRound[i].gameObject.SetActive(false);
			 }
			 for (int i = 0; i < _outerRound.Length; i++)
			 {
				_outerRound[i].gameObject.SetActive(false);
			 }
			 for (int i = 0; i < _stoneEyes.Length; i++)
			 {
				_stoneEyes[i].gameObject.SetActive(false);
			 }
			 _groundMaterial.SetColor("_EmissionColor1", Color.black);
			 _groundMaterial.SetColor("_EmissionColor2", Color.black);
			 _doorLight.gameObject.SetActive(false);
			 _doorLeft.rotation = Quaternion.Euler(0, 0, 0);
			 _doorRight.rotation = Quaternion.Euler(0, 0, 0);
		  }
		  if (State >= 1)
		  {
			 for (int i = 0; i < _innerRound.Length; i++)
			 {
				_innerRound[i].gameObject.SetActive(true);
			 }
			 _groundMaterial.SetColor("_EmissionColor1", _emis1);
			 _doorLight.gameObject.SetActive(false);
			 _doorLeft.rotation = Quaternion.Euler(0, 0, 0);
			 _doorRight.rotation = Quaternion.Euler(0, 0, 0);
		  }
		  if (State >= 2)
		  {
			 for (int i = 0; i < _outerRound.Length; i++)
			 {
				_outerRound[i].gameObject.SetActive(true);
			 }
			 for (int i = 0; i < _stoneEyes.Length; i++)
			 {
				_stoneEyes[i].gameObject.SetActive(true);
			 }
			 _doorLight.gameObject.SetActive(true);
			 _groundMaterial.SetColor("_EmissionColor2", _emis2);
			 _doorLeft.rotation = Quaternion.Euler(0, 75, 0);
			 _doorRight.rotation = Quaternion.Euler(0, -75, 0);
		  }
		}

	 }
	 protected override void Start()
	 {
		base.Start();

		for(int i = 0; i < _fireBowl.Length; i++)
		{
		  _fireBowl[i].OnFire = OnFiredBowl;
		}

	 }

	 protected override void LoadData(JValue data)
	 {
		base.LoadData(data);

		JArray bowls = data as JArray;
		for (int i = 0; i < _fireBowl.Length; i++)
		{
		  _fireBowl[i].IsFire = bowls.GetBool(i);
		}

	 }

	 protected override JValue SaveData()
	 {
		JArray bowls = new JArray();
		for (int i = 0; i < _fireBowl.Length; i++)
		{
		  bowls.Add(_fireBowl[i].IsFire);
		}

		return bowls;
	 }

	 private void OnFiredBowl()
	 {
		if (!FullBowles())
		  return;

		State = 1;
		Save();

		for(int i = 0; i < _innerRound.Length; i++)
		{
		  float intensity = _innerRound[i].intensity;
		  _innerRound[i].intensity = 0;
		  _innerRound[i].gameObject.SetActive(true);
		  _innerRound[i].DOIntensity(intensity, 0.5f);
		}

		DOTween.To(() => _groundMaterial.GetColor("_EmissionColor1"), (x) => _groundMaterial.SetColor("_EmissionColor1", x), _emis1, 0.5f);

	 }

	 public void CompleteRolls()
	 {
		State = 2;
		Save();

		DOTween.To(() => _groundMaterial.GetColor("_EmissionColor2"), (x) => _groundMaterial.SetColor("_EmissionColor2", x), _emis2, 0.5f);

		for (int i = 0; i < _outerRound.Length; i++)
		{
		  float intensity = _outerRound[i].intensity;
		  _outerRound[i].intensity = 0;
		  _outerRound[i].gameObject.SetActive(true);
		  _outerRound[i].DOIntensity(intensity, 0.5f);
		}
		for (int i = 0; i < _stoneEyes.Length; i++)
		{
		  float intensity = _stoneEyes[i].intensity;
		  _stoneEyes[i].intensity = 0;
		  _stoneEyes[i].gameObject.SetActive(true);
		  _stoneEyes[i].DOIntensity(intensity, 0.5f);
		}
		float portalintensity = _doorLight.intensity;
		_doorLight.intensity = 0;
		_doorLight.gameObject.SetActive(true);
		_doorLight.DOIntensity(portalintensity, 0.2f);
		_doorLeft.DORotate(new Vector3(0, 75, 0), 0.5f);
		_doorRight.DORotate(new Vector3(0, -75, 0), 0.5f);

	 }

	 private bool FullBowles()
	 {
		for (int i = 0; i < _fireBowl.Length; i++)
		{
		  if (!_fireBowl[i].IsFire)
			 return false;
		}
		return true;
	 }


  }
}