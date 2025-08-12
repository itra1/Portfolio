using UnityEngine;
using System.Collections;

namespace it.Game.Utils
{
  public class ObiFualBase : MonoBehaviour
  {
	 Obi.ObiParticleRenderer _rendere;
	 private void Start()
	 {
		_rendere = GetComponentInChildren<Obi.ObiParticleRenderer>();
		CameraBehaviour.Instance.AddFluedRendere(_rendere);
	 }

	 private void OnDestroy()
	 {
		if(_rendere != null)
		CameraBehaviour.Instance.RemoveFluedRendere(_rendere);
	 }
  }
}