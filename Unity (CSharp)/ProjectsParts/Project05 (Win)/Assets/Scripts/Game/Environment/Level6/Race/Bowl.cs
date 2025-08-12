using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level6.Race
{
  public class Bowl : MonoBehaviourBase
  {
	 [SerializeField]
	 private ParticleSystem[] _paticles;
	 [SerializeField]
	 private Light _light;

	 private bool _isActive;

	 public void Activate(bool isActive, bool force = false)
	 {

		if (!force && _isActive == isActive)
		  return;

		_isActive = isActive;

		if (_isActive)
		{
		  for(int i = 0;i < _paticles.Length; i++)
		  {
			 var part = _paticles[i];
			 var emis =  part.emission;
			 emis.enabled = true;
		  }

		  _light.DOIntensity(2, 0.5f);
		}
		else
		{
		  for (int i = 0; i < _paticles.Length; i++)
		  {
			 var part = _paticles[i];
			 var emis = part.emission;
			 emis.enabled = false;
		  }
		  _light.DOIntensity(0, 0.5f);

		}

	 }

  }
}