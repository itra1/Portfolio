using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.All.Traps
{
  public class BladeTraps : MonoBehaviourBase
  {
	 public bool IsActive
	 {
		get => _isActive; set
		{
		  if (_isActive == value)
			 return;
		  _isActive = value;

		  if (_isActive && _activeCor == null)
			 _activeCor = StartCoroutine(ActiveCoroutine());

		}
	 }
	 [SerializeField]
	 private bool _isActive = true;
	 [SerializeField]
	 private Collider _collider;
	 [SerializeField]
	 private Transform _rotator;
	 /// <summary>
	 /// Время между атаками
	 /// </summary>
	 [SerializeField]
	 private float _period = 2;
	 [SerializeField]
	 private float _durationHit = 1;
	 [SerializeField]
	 private float _durationBack = 1;
	 /// <summary>
	 /// Ожидание в середине
	 /// </summary>
	 [SerializeField]
	 private float _middleWait = 1;

	 [ContextMenu("SetActive")]
	 public void SetActive()
	 {
		IsActive = true;
	 }
	 [ContextMenu("SetActive")]
	 public void SetDisActive()
	 {
		IsActive = false;
	 }

	 private void Start()
	 {
		_collider.enabled = false;
		_activeCor = StartCoroutine(ActiveCoroutine());
	 }
	 Coroutine _activeCor;
	 IEnumerator ActiveCoroutine()
	 {
		while (_isActive)
		{
		  _collider.enabled = true;
		  Tween tween = _rotator.DOLocalRotate(new Vector3(85, 0, 0), _durationHit, RotateMode.Fast);
		  yield return tween.WaitForCompletion();
		  _collider.enabled = false;
		  yield return new WaitForSeconds(_middleWait);
		  tween = _rotator.DOLocalRotate(new Vector3(0, 0, 0), _durationBack, RotateMode.Fast);
		  yield return tween.WaitForCompletion();
		  yield return new WaitForSeconds(_period);
		}

	 }

  }
}
