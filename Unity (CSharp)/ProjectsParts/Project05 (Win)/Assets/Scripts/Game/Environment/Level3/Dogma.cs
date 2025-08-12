using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace it.Game.Environment.Level3
{
  public class Dogma : MonoBehaviourBase
  {
	 [SerializeField]
	 private ParticleSystem _particles;

	 [SerializeField]
	 private Light[] _lights;
	 [SerializeField]
	 private Material toothMaterial;
	 [SerializeField]
	 [ColorUsage(false,true)]
	 private Color activeColor;
	 

	 public void Activate()
	 {
		// State = 2;
		//ConfirmState();
		//Save();

		_particles.Play();

		DOTween.To(() => toothMaterial.GetColor("_EmissionColor"), (x) => toothMaterial.SetColor("_EmissionColor", activeColor), activeColor, 2);
		
		for (int i = 0; i < _lights.Length; i++)
		{
		  _lights[i].gameObject.SetActive(false);
		  _lights[i].intensity = 0;
		  _lights[i].DOIntensity(3, 2f);

		}
	 }

	 public void Deactivate()
	 {

		_particles.Stop();
		toothMaterial.SetColor("_Emission", Color.black);
		for (int i = 0; i < _lights.Length; i++)
		{
		  _lights[i].gameObject.SetActive(false);

		}
	 }

  }
}