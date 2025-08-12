using UnityEngine;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using it.Game.Managers;
using DG.Tweening;

namespace it.Game.PlayMaker
{
  public class LightingItemsLightItems : FsmStateAction
  {
	 public FsmGameObject particlePrefab;
	 public FsmEvent _onComplete;

	 public string[] _items;
	 private int _state = 0;
	 private List<Items.Inventary.InventaryItem> currentItems = new List<Items.Inventary.InventaryItem>();

	 public override void OnEnter()
	 {
		currentItems.Clear();

		for (int i = 0; i < _items.Length; i++)
		{
		  for (int j = 0; j < GameManager.Instance.LocationManager.InventaryItems.Length; j++)
		  {
			 if (_items[i] == GameManager.Instance.LocationManager.InventaryItems[j].Uuid)
				currentItems.Add(GameManager.Instance.LocationManager.InventaryItems[j]);
		  }
		}


		for(int i = 0; i < currentItems.Count; i++)
		{
		  GameObject effect = MonoBehaviour.Instantiate(particlePrefab.Value, currentItems[i].transform.position, Quaternion.identity);

		  int index = i;

		  ParticleSystem ps = effect.GetComponentInChildren<ParticleSystem>();
		  ps.Stop();
		  ParticleSystem.EmissionModule em = ps.emission;
		  em.enabled = false;
		  ps.Play();
		  em.enabled = true;

		  DOVirtual.DelayedCall(4f, () =>
		  {
			 em.enabled = false;
			 DOVirtual.DelayedCall(1f, () =>
			 {
				MonoBehaviour.Destroy(effect, 1);
				if(index >= currentItems.Count-1)
				{
				  Fsm.Event(_onComplete);
				}
			 });
		  });

		}

	 }

  }
}