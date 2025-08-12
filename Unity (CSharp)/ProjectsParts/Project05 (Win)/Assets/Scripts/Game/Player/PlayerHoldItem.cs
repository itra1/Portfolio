using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Game.Items;

namespace it.Game.Player
{
  public class PlayerHoldItem : PlayerComponent
  {
	 [SerializeField]
	 private Transform _LeftHand;
	 [SerializeField]
	 private Transform _RightHand;

	 /// <summary>
	 /// Левая рука
	 /// </summary>
	 public Transform LeftHand { get => _LeftHand; }
	 /// <summary>
	 /// Правая рука
	 /// </summary>
	 public Transform RightHand { get => _RightHand; }

	 /// <summary>
	 /// Удерживается предмет в левой руке
	 /// </summary>
	 public bool LeftHold { get => _LeftItem != null; }
	 /// <summary>
	 /// Удерживается предмет в правой руке
	 /// </summary>
	 public bool RightHold { get => _RightItem != null; }

	 private HoldItem _LeftItem;
	 private HoldItem _RightItem;

	 /// <summary>
	 /// Предмет в левой руке
	 /// </summary>
	 public HoldItem LeftItem { get => _LeftItem; }

	 /// <summary>
	 /// Предмет в правой руке
	 /// </summary>
	 public HoldItem RightItem { get => _RightItem; }

	 public void StartUse(HoldItem holdItem)
	 {
		bool isLeft = (holdItem.Hand & HoldHandType.left) != 0;

		holdItem.transform.SetParent(isLeft ? _LeftHand : _RightHand);

		if (isLeft)
		{
		  _LeftItem = holdItem;
		}
		if (!isLeft)
		{
		  _RightItem = holdItem;
		}

		holdItem.transform.localPosition = holdItem.LocalPosition;
		holdItem.transform.localEulerAngles =  holdItem.LocalRotation;
		holdItem.transform.localScale = holdItem.LocalScale;

	 }
	 public void StopUse(HoldItem holdItem)
	 {

		bool isLeft = (holdItem.Hand & HoldHandType.left) != 0;
		if (isLeft)
		{
		  _LeftItem = null;
		}
		if (!isLeft)
		{
		  _RightItem = null;
		}
	 }

	 private void Start()
	 {
		
	 }

  }
}