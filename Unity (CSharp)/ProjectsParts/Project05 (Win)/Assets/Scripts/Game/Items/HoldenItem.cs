using System;
using System.Collections;
using System.Collections.Generic;
using it.Game.Player.Interactions;
using UnityEngine;

namespace it.Game.Items
{
  [RequireComponent(typeof(InteractionObject))]
  public class HoldenItem : Item
  {
	 private InteractionObject _interactonObject;
	 private InteractionTarget[] _interactionTargets;
	 private InteractionTrigger _interactionTrigger;
	 private Rigidbody _rigidbody;
	 
	 protected bool _isHolded;

	 [SerializeField]
	 private HoldenAnimationType holdenAnimation;
	 public HoldenAnimationType HoldenAnimation { get => holdenAnimation; set => holdenAnimation = value; }

	 public enum HoldenAnimationType
	 {
		none,
		leftUp
	 }

	 protected bool _isUse;

	 public virtual int HoldStey => 0;


	 protected virtual void OnEnable()
	 {
		_interactonObject = GetComponent<InteractionObject>();
		if (_interactonObject == null)
		  _interactonObject = GetComponentInChildren<InteractionObject>(true);

		_interactionTargets = GetComponentsInChildren<InteractionTarget>(true);
		_interactionTrigger = GetComponentInChildren<InteractionTrigger>(true);
		_rigidbody = GetComponent<Rigidbody>();

	 }

	 public virtual void Use()
	 {
		_isUse = true;
	 }
	 public virtual void UnUse()
	 {
		_isUse = false;
	 }

	 /// <summary>
	 /// Подбираем
	 /// </summary>
	 [ContextMenu("Hold")]
	 public virtual void Hold()
	 {
		SetInterractionData(false);
	 }

	 /// <summary>
	 /// После интерации
	 /// </summary>
	 public virtual void AfterHold()
	 {
		Debug.Log("After hold item");
	 }

	 /// <summary>
	 /// Бросаем
	 /// </summary>
	 [ContextMenu("Drop")]
	 public virtual void Drop()
	 {
		SetInterractionData(true);
	 }
	 /// <summary>
	 /// Установка интерактивных данных
	 /// </summary>
	 /// <param name="isFree">Свободна</param>
	 protected virtual void SetInterractionData(bool isFree)
	 {
		_isHolded = !isFree;
		_interactonObject.enabled = isFree;
		_interactionTrigger.gameObject.SetActive(isFree);
		_interactionTrigger.transform.rotation = Quaternion.Euler(0, 90, 0);
		for (int i = 0; i < _interactionTargets.Length; i++)
		{
		  _interactionTargets[i].gameObject.SetActive(isFree);
		}

		GetComponent<Collider>().enabled = isFree;

		if (_rigidbody != null)
		{
		  _rigidbody.useGravity = isFree;
		  _rigidbody.isKinematic = !isFree;
		  _rigidbody.interpolation = isFree ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
		  _rigidbody.collisionDetectionMode = isFree ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
		}

	 }

  }
}