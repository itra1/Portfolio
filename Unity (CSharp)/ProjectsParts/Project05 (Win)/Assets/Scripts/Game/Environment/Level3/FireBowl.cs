using UnityEngine;
using System.Collections;
using it.Game.Player;
using DG.Tweening;

namespace it.Game.Environment.Level3
{
  public class FireBowl : MonoBehaviourBase, it.Game.Player.Interactions.IInteractionCondition
  {
	 public UnityEngine.Events.UnityAction OnFire;
	 [SerializeField]
	 private GameObject _fire;
	 [SerializeField]
	 private ParticleSystem _fireParticle;
	 [SerializeField]
	 private ParticleSystem _smokeParticle;
	 [SerializeField]
	 private ParticleSystem _lensParticles;
	 [SerializeField]
	 private Light _fireLight;
	 [SerializeField]
	 private bool _needFire;
	 public bool NeedFire { get => _needFire; set => _needFire = value; }
	 [SerializeField]
	 private bool _isFire;
	 public bool IsFire
	 {
		get => _isFire; 
		set
		{
		  //_isFire = value;
		  ActiveFire(value);
		  //_fire.SetActive(_isFire);

		}
	 }


	 public bool InteractionReady()
	 {
		var holdItem = PlayerBehaviour.Instance.GetHoldItem();
		if (holdItem == null)
		  return false;
		if (!holdItem.Uuid.Equals("d5be7a1a-4cbf-4eac-a897-8c38bb86e6f8"))
		  return false;

		var torch = holdItem.GetComponent<it.Game.Items.Torch>();

		if (torch == null)
		  return false;

		if (torch.IsFire && _needFire && !_isFire)
		  return true;
		if (!torch.IsFire && _isFire)
		  return true;

		return false;

	 }

	 public void Contact()
	 {
		var holdItem = PlayerBehaviour.Instance.GetHoldItem();
		if (holdItem == null)
		  return;
		if (!holdItem.Uuid.Equals("d5be7a1a-4cbf-4eac-a897-8c38bb86e6f8"))
		  return;

		var torch = holdItem.GetComponent<it.Game.Items.Torch>();
		if (IsFire)
		{
		  torch.SetFire();
		  return;
		}
		else
		{
		  if (NeedFire && !IsFire && torch.IsFire)
		  {
			 IsFire = true;
			 OnFire?.Invoke();
		  }
		}

	 }

	 private void ActiveFire(bool active)
	 {
		var fireEmis = _fireParticle.emission;
		var smokeEmis = _smokeParticle.emission;
		var lensEmis = _lensParticles.emission;

		if (active == _isFire)
		  return;

		_isFire = active;

		if (_isFire)
		{
		  fireEmis.enabled = false;
		  smokeEmis.enabled = false;
		  lensEmis.enabled = false;
		  var intencity = _fireLight.intensity;
		  _fireLight.intensity = 0;

		  _fire.gameObject.SetActive(_isFire);
		  _fireLight.DOIntensity(intencity, 1).SetDelay(0.3f);
		  fireEmis.enabled = true;
		  DOVirtual.DelayedCall(0.3f, () => {
			 smokeEmis.enabled = true;
		  });
		  DOVirtual.DelayedCall(1f, () => {
			 lensEmis.enabled = true;
		  });

		}
		else
		{
		  fireEmis.enabled = false;
		  smokeEmis.enabled = false;
		  lensEmis.enabled = false;

		  DOVirtual.DelayedCall(3f, () => {
			 _fire.gameObject.SetActive(_isFire);
		  });
		}

	 }


  }
}