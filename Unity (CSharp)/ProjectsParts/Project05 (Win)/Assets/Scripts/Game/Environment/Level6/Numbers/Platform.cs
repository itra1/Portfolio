using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level6.Numbers
{
  public class Platform : MonoBehaviourBase
  {
	 [SerializeField]
	 private Transform _block;

	 private bool _isHide;

	 public void SetHidePlatform(bool isHide)
	 {
		if (_isHide == isHide)
		  return;

		if (isHide)
		  _block.DOLocalMoveY(-5, 2);
		else
		  _block.localPosition = Vector3.zero;

	 }

#if UNITY_EDITOR

	 //private void OnDrawGizmosSelected()
	 //{
		//if (_block == null)
		//  _block = GetComponentInChildren<Transform>();
	 //}

	 [ContextMenu("Change Parent")]
	 private void ChangeParent()
	 {
		gameObject.name = "Platform";

		Transform parent = transform.parent;
		_block = parent;
		transform.SetParent(parent.parent);
		transform.localScale = Vector3.one;
		parent.SetParent(transform);
	 }

#endif

  }
}