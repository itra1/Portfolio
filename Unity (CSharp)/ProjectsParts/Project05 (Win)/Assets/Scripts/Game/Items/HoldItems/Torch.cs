using UnityEngine;
using System.Collections;
using UnityEditor;
using DG.Tweening;

namespace it.Game.Items
{
  public class Torch : HoldenItem
  {
    [SerializeField]
    private GameObject _fire;
    [SerializeField]
    private ParticleSystem _fireParticle;
    [SerializeField]
    private ParticleSystem _smokeParticle;
    [SerializeField]
    private ParticleSystem _lensParticles;

    /// <summary>
    /// Горит
    /// </summary>
    [SerializeField]
    private bool _isFire = false;

    public bool IsFire { get => _isFire; set => _isFire = value; }

    protected override void OnEnable()
    {
      base.OnEnable();

      ActiveFire(IsFire);
      //_fire.gameObject.SetActive(IsFire);
    }

    /// <summary>
    /// Поджечь
    /// </summary>
    public void SetFire()
    {
      ActiveFire(true);
      //_fire.SetActive(true);
    }

    public void SnuffOut()
    {
      ActiveFire(false);
      //_fire.SetActive(false);
    }

    public override void Drop()
    {
      base.Drop();
      ActiveFire(false);
     // _fire.SetActive(false);
    }

    private void ActiveFire(bool active)
	 {
      var fireEmis = _fireParticle.emission;
      var smokeEmis = _smokeParticle.emission;
      var lensEmis = _lensParticles.emission;

      if (active == IsFire)
        return;
      
      IsFire = active;

		if (IsFire)
		{
        fireEmis.enabled = false;
        smokeEmis.enabled = false;
        lensEmis.enabled = false;

        _fire.gameObject.SetActive(IsFire);

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
          _fire.gameObject.SetActive(IsFire);
        });
      }

    }

  }
}