using UnityEngine;
using System.Collections;
using it.Game.Player.Interactions;
using it.Game.Handles;
using DG.Tweening;

namespace it.Game.Environment.Level5.ThreeObelisks
{
  public class CrystalPostament : Environment, IInteractionCondition
  {
	 [SerializeField]
	 private string uuidItem;
	 [SerializeField]
	 private GameObject _crystal;
	 [SerializeField]
	 private Light _light;
	 [SerializeField]
	 private ParticleSystem _particles;

	 [SerializeField]
	 private DarkTonic.MasterAudio.AmbientSound _ambientSound;

	 private bool _isExist;

	 public bool IsExist { get => _isExist; set => _isExist = value; }

	 private float _startIntensityLight;
	 protected override void Awake()
	 {
		base.Awake();
		_startIntensityLight = _light.intensity;
	 }

	 public void Interaction()
	 {
		State = 2;
		Game.Managers.GameManager.Instance.Inventary.AddItem(uuidItem, 1);
		_ambientSound.enabled = false;
		DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransformAndForget("CollectItems", _crystal.transform, 1, null, 0, "CollectItem1");
		Save();
		ConfirmState();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		LightImpulsing impulsing = _light.GetComponent<LightImpulsing>();
		var emiss = _particles.emission;

		if (State == 2)
		{
		  _crystal.SetActive(false);
		  impulsing.IsActived = false;
		  emiss.enabled = false;
		  _light.DOIntensity(0, 0.5f);
		  _ambientSound.enabled = false;
		}
		else
		{
		  _crystal.SetActive(true);
		  emiss.enabled = true;
		  impulsing.IsActived = false;
		  _light.intensity = _startIntensityLight;
		  impulsing.IsActived = true;
		  _ambientSound.enabled = true;
		}

	 }


	 public bool InteractionReady()
	 {
		return State < 2;
	 }
  }
}