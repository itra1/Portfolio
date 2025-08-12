using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level5.PriestArena
{
  public class PriestLaserBullet : MonoBehaviourBase
  {
	 private PriestEyeLight[] _lights;

	 private void Start()
	 {
		_lights = GetComponentsInChildren<PriestEyeLight>(true);
		Clear();
	 }

	 private void Clear()
	 {
		for (int i = 0; i < _lights.Length; i++)
		{
		  _lights[i].gameObject.SetActive(false);
		}
	 }

	 public void Shoot()
	 {
		for(int i = 0; i < _lights.Length; i++)
		{
		  _lights[i].gameObject.SetActive(true);
		}
	 }

  }
}