using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
namespace it.Game.NPC.Enemyes.Boses.Karmamancer
{
  public class KarmamancerMovePosition : MonoBehaviour
  {
	 [SerializeField]
	 private GameObject _fog;

	 private List<GameObject> _objects = new List<GameObject>();

	 public void Spawn(Vector3 targetPostion)
	 {
		GameObject Obj = GetInstance();
		Obj.transform.position = targetPostion;
		PositionFog(Obj, targetPostion);
	 }

	 private void PositionFog(GameObject obj, Vector3 targetPosition)
	 {
		obj.gameObject.SetActive(true);
		ParticleSystem particle = obj.GetComponentInChildren<ParticleSystem>();
		ParticleSystem.EmissionModule emiss = particle.emission;
		particle.Play();
		emiss.enabled = true;
		DOVirtual.DelayedCall(4, () =>
		{
		  emiss.enabled = false;
		});
		DOVirtual.DelayedCall(10, () =>
		{
		  obj.gameObject.SetActive(false);
		});
	 }

	 private GameObject GetInstance()
	 {
		GameObject elem = _objects.Find(x => !x.activeInHierarchy);

		if (elem == null)
		{
		  elem = Instantiate(_fog, _fog.transform.parent);
		  _objects.Add(elem);

		}

		return elem;
	 }

  }
}