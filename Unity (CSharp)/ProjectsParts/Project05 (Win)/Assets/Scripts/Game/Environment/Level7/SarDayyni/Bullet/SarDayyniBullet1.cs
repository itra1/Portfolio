using UnityEngine;
using System.Collections;
using it.Game.NPC.Enemyes.Boses.SarDayyni;
using DG.Tweening;

namespace it.Game.Environment.Level7.SarDayyni
{
  public class SarDayyniBullet1 : MonoBehaviourBase
  {
	 [SerializeField] private Transform _graph;
	 [SerializeField] private GameObject _bullet;

	 private int _count = 6;
	 private void OnEnable()
	 {
		_bullet.SetActive(false);
		_graph.gameObject.SetActive(true);
		_graph.localScale = Vector3.zero;
		_graph.DOScale(1, 1f);
		DOVirtual.DelayedCall(20, ()=> {

		  Destroy(gameObject);

		});
		DOVirtual.DelayedCall(2, GenerateInctances);
		DOVirtual.DelayedCall(6, () =>
		{
		  gameObject.SetActive(false);
		});
	 }

	 private void GenerateInctances()
	 {
		_graph.gameObject.SetActive(false);
		for (int i = 0; i < _count; i++)
		{
		  GameObject inst = Instantiate(_bullet, transform.position, Quaternion.identity);
		  inst.transform.parent = null;
		  inst.gameObject.SetActive(true);
		  Destroy(inst, 10);
		}
	 }

  }
}