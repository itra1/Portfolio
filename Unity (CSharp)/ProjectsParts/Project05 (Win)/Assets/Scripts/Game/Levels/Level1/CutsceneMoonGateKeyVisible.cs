using DG.Tweening;
using it.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Levels.Level1
{
	public class CutsceneMoonGateKeyVisible : MonoBehaviour
	{
		[SerializeField] private Environment.Handlers.PegasusController _pegasus;
		[SerializeField] private GameObject _particlePrefab;
		[SerializeField] private string[] _items;

		private List<Items.Inventary.InventaryItem> currentItems = new List<Items.Inventary.InventaryItem>();

		[ContextMenu("Play")]
		public void Play()
		{
			_pegasus.FromCameraPosition = true; ;
			_pegasus.Activate(() =>
			{
				VisibleItems(() =>
				{
					_pegasus.Deactivate();
				});

			});

		}

		private void VisibleItems(UnityEngine.Events.UnityAction OnComplete){

			currentItems.Clear();

			for (int i = 0; i < _items.Length; i++)
			{
				for (int j = 0; j < GameManager.Instance.LocationManager.InventaryItems.Length; j++)
				{
					if (_items[i] == GameManager.Instance.LocationManager.InventaryItems[j].Uuid)
						currentItems.Add(GameManager.Instance.LocationManager.InventaryItems[j]);
				}
			}


			for (int i = 0; i < currentItems.Count; i++)
			{
				GameObject effect = MonoBehaviour.Instantiate(_particlePrefab, currentItems[i].transform.position, Quaternion.identity);

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
						if (index >= currentItems.Count - 1)
						{
							OnComplete?.Invoke();
						}
					});
				});

			}
		}


	}
}