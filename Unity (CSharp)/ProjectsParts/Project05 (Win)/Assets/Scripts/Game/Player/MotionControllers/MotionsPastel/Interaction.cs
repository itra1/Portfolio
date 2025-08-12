using UnityEngine;
using com.ootii.Actors;
using com.ootii.Actors.AnimationControllers;
using it.Game.Items;
using System.Collections;
using System.Collections.Generic;
using com.ootii.Geometry;
using com.ootii.Helpers;
using it.Game.Player.Interactions;
using it.Game.Managers;

#if UNITY_EDITOR
using UnityEditor;
using com.ootii.Graphics;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Interaction Pastel")]
  [MotionDescription("Interaction wish Objects. Use Items")]
  public class Interaction : MotionControllerMotion
  {
	 public int PHASE_UNKNOWN = 0;
	 public int PHASE_START = 3450;
	 public int PHASE_LOW = 3451;
	 public int PHASE_MIDDLE = 3452;
	 public int PHASE_HIGHT = 3453;
	 public int PHASE_CONTINUE = 3455;

	 private int _animatorCurrent;

	 public bool _IsInteractableRaycastEnabled = true;
	 public bool IsInteractableRaycastEnabled
	 {
		get { return _IsInteractableRaycastEnabled; }
		set { _IsInteractableRaycastEnabled = value; }
	 }
	 public string _WalkRunMotion = "Controlled Walk";
	 public string WalkRunMotion
	 {
		get { return _WalkRunMotion; }
		set { _WalkRunMotion = value; }
	 }

	 /// <summary>
	 /// Form that is used for this activation
	 /// </summary>
	 protected int mActiveForm = -1;
	 public int ActiveForm
	 {
		get { return mActiveForm; }
		set { mActiveForm = value; }
	 }
	 private float _radiusCheck = 0.15f;
	 public float RadiusCheck { get => _radiusCheck; set => _radiusCheck = value; }

	 private float _distanceCheck = 1f;
	 public float DistanceCheck { get => _distanceCheck; set => _distanceCheck = value; }

	 public int _InteractableLayers = -1;
	 public int InteractableLayers
	 {
		get { return _InteractableLayers; }
		set { _InteractableLayers = value; }
	 }
	 public float _WalkSpeed = 1f;
	 public float WalkSpeed
	 {
		get { return _WalkSpeed; }
		set { _WalkSpeed = value; }
	 }

	 private it.Game.Player.MotionControllers.Motions.HoldItem _holdenItemMotion;

	 /// <summary>
	 /// Speed to rotate to the target location
	 /// </summary>
	public float _RotationSpeed = 180f;
	 public float RotationSpeed
	 {
		get { return _RotationSpeed; }
		set { _RotationSpeed = value; }
	 }

	 private Interactions.InteractionObject _forceInteraction;
	 public Interactions.InteractionObject ForceInteraction
	 {
		get { return _forceInteraction; }
		set { _forceInteraction = value; }
	 }
	 private Interactions.InteractionTarget _forceInteractionTarget;
	 public Interactions.InteractionTarget ForceInteractionTarget
	 {
		get { return _forceInteractionTarget; }
		set { _forceInteractionTarget = value; }
	 }

	 private int _itemLayer = -1;
	 public int ItemLayer { get => _itemLayer; set => _itemLayer = value; }


	 private IInteraction _item = null;
	 public IInteraction Item { get => _item; set => _item = value; }
	 public bool IsGetItem { get => _isGetItem; set => _isGetItem = value; }

	 private StateIteractionType _state = StateIteractionType.none;

	 private List<ILightItem> _lightingItemBefore = new List<ILightItem>();
	 private List<ILightItem> _lightingItemAfter = new List<ILightItem>();

	 private PlayerBehaviour _playerBehaviour;
	 private Interactions.InteractionMotion _interactionMotion;

	 //private it.Game.Items.HoldenItem _holdenItem;
	 //public HoldenItem HoldItem { get => _holdenItem; set => _holdenItem = value; }


	 private bool _isGetItem = false;
	 private int _closestTriggerIndex = 0;

	 private bool mIsMovingToTarget;

	 [System.Flags]
	 private enum StateIteractionType
	 {
		none = 0,
		activeAnimation = 1,
		completeAnimation = 2,
		getItem = 4
	 }

	 public Interaction()
	 : base()
	 {
		_Category = EnumMotionCategories.INTERACT;

		_Priority = 20;
		_ActionAlias = "Use";
		_Form = 0;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Interaction-SM"; }
#endif
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public Interaction(MotionController rController)
		  : base(rController)
	 {
		_Category = EnumMotionCategories.INTERACT;

		_Priority = 20;
		_ActionAlias = "Use";
		_Form = 0;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Interaction-SM"; }
#endif
	 }

	 public override bool TestActivate()
	 {
		if (!mIsStartable) { return false; }
		_targetTrigger = null;
		_targetRange = null;

		if (_playerBehaviour == null)
		{
		  _playerBehaviour = mMotionController.GetComponent<PlayerBehaviour>();
		  _playerBehaviour.InteractionSystem.OnInteractionStart += OnInteractionStart;
		  _playerBehaviour.InteractionSystem.OnInteractionStop += OnInteractionStop;
		  _playerBehaviour.InteractionSystem.OnInteractionPickUp += OnInteractionPickUp;
		  _playerBehaviour.InteractionSystem.OnInteractionEvent += OnInteractionEvent;
		  _playerBehaviour.InteractionSystem.OnInteractionResume += OnInteractionOnResume;
		  _playerBehaviour.FullBodyBipedIK.solver.OnPreUpdate += OnPreFBBIK;
		  _playerBehaviour.FullBodyBipedIK.solver.OnPostUpdate += OnPostFBBIK;
		  _playerBehaviour.FullBodyBipedIK.solver.OnFixTransforms += OnFixTransforms;

		  _holdenItemMotion = mMotionController.GetMotion<HoldItem>();

		  if (_holdenItemMotion == null)
			 throw new System.Exception("У плеера отсутсвует компонент HoldItem");
		}

		if (Form >= 0 && Form != mMotionController.CurrentForm)
		  return false;

		SetBackLight();


		_closestTriggerIndex = _playerBehaviour.InteractionSystem.GetClosestTriggerIndex();



		if (_closestTriggerIndex == -1 && _holdenItemMotion.IsHold && mMotionController._InputSource.IsJustPressed(_ActionAlias))
		{
		  DropHoldItemIfExists();
		  return false;
		}

		if (_closestTriggerIndex == -1)
		{
		  SetVisibleIndicator(null);
		  //DropHoldItemIfExists();
		  return false;
		}

		if (!_playerBehaviour.InteractionSystem.TriggerEffectorsReady(_closestTriggerIndex))
		{
		  //DropHoldItemIfExists();
		  SetVisibleIndicator(null);
		  return false;
		}

		var iterObject = _playerBehaviour.InteractionSystem.GetClosestInteractionObjectInRange();

		//var itemConditions = iterObject.GetComponents<it.Game.Player.Interactions.IInteractionCondition>();

		bool isInteract = IsInteructReady(iterObject);

		if (!isInteract)
		{
		  if (_playerBehaviour.InteractionSystem.triggersInRange.Count <= 1)
		  {
			 SetVisibleIndicator(null);
			 return false;
		  }

		  bool isExists = false;
		  for(int i = 0; i < _playerBehaviour.InteractionSystem.triggersInRange.Count; i++)
		  {
			 if (isExists)
				continue;
			 //iterObject = _playerBehaviour.InteractionSystem.GetClosestInteractionObjectsInRange()[i];
			 _playerBehaviour.InteractionSystem.ContactIsInRange(_playerBehaviour.InteractionSystem.triggersInRange[i], out int bestRangeIndex);
			 iterObject = _playerBehaviour.InteractionSystem.triggersInRange[i].ranges[bestRangeIndex].interactions[0].interactionObject;

			 isInteract = IsInteructReady(iterObject);

			 if (isInteract)
			 {
				_targetRange = _playerBehaviour.InteractionSystem.triggersInRange[i].ranges[bestRangeIndex];
				_targetTrigger = _playerBehaviour.InteractionSystem.triggersInRange[i];
				break;
			 }
		  }
		  if (!isExists)
		  {
			 SetVisibleIndicator(null);
			 return false;
		  }
		}

		SetVisibleIndicator(iterObject);

		if (!mMotionController._InputSource.IsJustPressed(_ActionAlias))
		  return false;



		if (iterObject.OffsetUse != Interactions.InteractionObject.OffsetUseType.none && iterObject.InteractionVariant == InteractionObject.InteractionType.finalIK)
		{

		  mMotionController.StartCoroutine(MoveToTargetInternal(iterObject));
		  return false;
		}

		  return true;
	 }

	 /// <summary>
	 /// Проверка на возмоность взаимодействия
	 /// </summary>
	 /// <param name="inderObject"></param>
	 /// <returns></returns>
	 private bool IsInteructReady(InteractionObject inderObject)
	 {
		var itemConditions = inderObject.GetComponents<it.Game.Player.Interactions.IInteractionCondition>();

		bool isInteract = true;

		if (itemConditions.Length > 0)
		{
		  for (int i = 0; i < itemConditions.Length; i++)
		  {
			 if (!itemConditions[i].InteractionReady())
				isInteract = false;
		  }
		}
		return isInteract;
	 }

	 private void SetVisibleIndicator(InteractionObject interactTarget)
	 {
		if (interactTarget == _visibleIterObject)
		  return;

		_visibleIterObject = interactTarget;

		var panel =  UiManager.GetPanel<it.UI.Game.GameUI>();

		if(_visibleIterObject == null)
		{
		  panel.SetVisibleInteractButton(null);
		  return;
		}
		else
		{
		  panel.SetVisibleInteractButton(_visibleIterObject.transform, _visibleIterObject.IndicationOffset);
		  com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.InteractionFocus);
		}

	 }

	 private InteractionObject _visibleIterObject = null;

	 private InteractionTrigger _targetTrigger;
	 private InteractionTrigger.Range _targetRange;

	 public override bool TestUpdate()
	 {
		if (_isCustomAnim)
		{
		  if (IsInMotionState)
		  {
			 return true;
		  }
		  else
		  {
			 _isCustomAnim = false;
			 return false;
		  }
		}
		if (iterObject == null)
		  return false;

		if (iterObject.InteractionVariant == InteractionObject.InteractionType.none)
		  return false;

		if (_state == StateIteractionType.completeAnimation && !_isGetItem)
		  return false;

		return true;
	 }
	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {


		//float iteractionValue = mMotionController.Animator.GetFloat(PlayerBehaviour.ANIM_CRV_ITERACTION);
		//_interactionMotion.SetWeight(iteractionValue);


	 }

	 private bool _isCustomAnim;

	 private void SetBackLight()
	 {
		RaycastHit[] hits;
		int hitCount = RaycastExt.SafeSphereCastAll(mActorController._Transform.position, mActorController._Transform.forward, 70, out hits, 1, ItemLayer);

		if (hitCount == 0 && _lightingItemBefore.Count == 0)
		  return;
		_lightingItemBefore = new List<ILightItem>(_lightingItemAfter);
		_lightingItemAfter.Clear();

		for (int i = 0; i < hitCount; i++)
		{
		  ILightItem itm = hits[i].collider.GetComponent<ILightItem>();
		  if (itm != null)
			 _lightingItemAfter.Add(itm);
		}

		for (int i = 0; i < _lightingItemAfter.Count; i++)
		{
		  _lightingItemAfter[i].SetLight(((_lightingItemAfter[i] as Component).transform.position - mActorController._Transform.position).sqrMagnitude > (5 * 5));
		}
		for (int i = 0; i < _lightingItemBefore.Count; i++)
		{
		  if (!_lightingItemAfter.Contains(_lightingItemBefore[i]))
			 _lightingItemBefore[i].SetLight(false);
		}
	 }
	 Interactions.InteractionObject iterObject;
	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		_isGetItem = false;
		// Trigger the transition
		mActiveForm = (mActiveForm >= 0 ? mActiveForm : _Form);
		iterObject = null;
		Interactions.InteractionTarget target = null;
		if (ForceInteraction != null)
		{
		  iterObject = ForceInteraction;
		  target = ForceInteractionTarget;
		  ForceInteractionTarget = null;
		  ForceInteraction = null;
		}
		else
		{
		  iterObject = _playerBehaviour.InteractionSystem.GetClosestInteractionObjectsInRange()[_closestTriggerIndex];
		  target = _playerBehaviour.InteractionSystem.GetClosestInteractionTargetInRange();
		}

		SetVisibleIndicator(null);

		switch (iterObject.InteractionVariant)
		{
		  case Interactions.InteractionObject.InteractionType.finalIK:
			 {
				if(_targetRange != null)
				{
				  if (!_playerBehaviour.InteractionSystem.TriggerInteraction(_targetRange, false))
				  {
					 return false;
				  }
				}
				else
				{

				  if (!_playerBehaviour.InteractionSystem.TriggerInteraction(_closestTriggerIndex, false))
				  {
					 return false;
				  }
				}

				break;
			 }
		  case Interactions.InteractionObject.InteractionType.up:
			 _isCustomAnim = true;
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_HIGHT, mActiveForm, Parameter, true);
			 break;
		  case Interactions.InteractionObject.InteractionType.middle:
			 _isCustomAnim = true;
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_MIDDLE, mActiveForm, Parameter, true);
			 break;
		  case Interactions.InteractionObject.InteractionType.down:
			 _isCustomAnim = true;
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_LOW, mActiveForm, Parameter, true);
			 break;
		  case Interactions.InteractionObject.InteractionType.none:
			 {
				iterObject.events[0].unityEvent?.Invoke();
				break;
			 }
		}

		_item = iterObject.GetComponent<IInteraction>();

		var holdItm = iterObject.GetComponent<HoldenItem>();

		if (holdItm != null)
		{
		  DropHoldItemIfExists();
		}

		//mMotionController.Animator.

		//float deltaHeight = Mathf.Abs(target.transform.position.y - mActorController.transform.position.y);

		//if (deltaHeight <= 0.6f)
		//{
		//  Debug.Log("Player PHASE_LOW");
		//  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_LOW, true);
		//}
		//else if (deltaHeight <= 1.2f)
		//{
		//  Debug.Log("Player PHASE_MIDDLE");
		//  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_MIDDLE, true);
		//}
		//else
		//{
		//  Debug.Log("Player PHASE_HIGHT");
		//  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_HIGHT, true);
		//}

		if (iterObject == null)
		{
		  _item.StartInteract();
		  return base.Activate(rPrevMotion);
		}

		// Finalize the activation
		return base.Activate(rPrevMotion);
	 }
	 public override void Deactivate()
	 {
		iterObject = null;
	 }

	 public override void OnAnimationEvent(AnimationEvent rEvent)
	 {
		Debug.Log(rEvent);
		if(rEvent.stringParameter == "Interaction")
		{
		  if(iterObject.events.Length > 0)
			 iterObject.events[0].unityEvent.Invoke();
		}
	 }

	 private void OnInteractionStart(RootMotion.FinalIK.FullBodyBipedEffector effectorType, Game.Player.Interactions.InteractionObject interactionObject)
	 {
		it.Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = false;
		_state = StateIteractionType.activeAnimation;

		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.InteractionStart,0);
	 }
	 private void OnInteractionStop(RootMotion.FinalIK.FullBodyBipedEffector effectorType, Game.Player.Interactions.InteractionObject interactionObject)
	 {
		it.Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = true;
		_state = StateIteractionType.completeAnimation;
		if (_isGetItem && _item != null)
		{
		  var ii = (_item as Component).GetComponent<Game.Items.Inventary.InventaryItem>();

		  if (ii != null)
		  {
			 ii.ColorHide(() =>
			 {
				ii.GetItem();
				RemoveItem();
			 });
		  }

		}

		if (_holdenItemMotion.IsHold)
		{
		  _holdenItemMotion.HoldenItem.AfterHold();

		  //var holdItemMotion = mMotionController.GetMotion<it.Game.Player.MotionControllers.Motions.HoldItem>();


		  //_holdenItemMotion.SetItem(_holdenItem);

		  //Animator[] animators = mMotionController.GetComponentsInChildren<Animator>();
		  //for (int i = 0; i < animators.Length; i++)
		  //{
			 //animators[i].SetInteger("HoldItem", _holdenItem.HoldStey);
		  //}
		}

		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.InteractionStop, 0);

	 }
	 private void OnInteractionPickUp(RootMotion.FinalIK.FullBodyBipedEffector effectorType, Game.Player.Interactions.InteractionObject interactionObject)
	 {
		//IPickUpItemFromInventary pickUp = interactionObject.GetComponent<IPickUpItemFromInventary>();
		//if (pickUp != null)
		//{

		//  var effector = _playerBehaviour.FullBodyBipedIK.solver.GetEffector(effectorType);

		//  string itemPickUp = pickUp.PickUpItem;
		//  interactionObject.transform.SetParent(null);

		//  Vector3 parentPosition = effector.bone.position;
		//  Quaternion rotation = effector.bone.rotation;

		//  var notePrefab = it.Game.Managers.GameManager.Instance.Inventary.GetPrefab(itemPickUp);
		//  GameObject inst = it.Game.Managers.GameManager.Instantiate(notePrefab);
		//  var target = inst.GetComponentInChildren<it.Game.Player.Interactions.InteractionTarget>();
		//  effector.bone.transform.position = target.transform.position;
		//  effector.bone.transform.rotation = target.transform.rotation;
		//  inst.transform.SetParent(effector.bone.transform);
		//  effector.bone.transform.position = parentPosition;
		//  effector.bone.transform.rotation = rotation;
		//  effector.bone.GetComponent<RootMotion.FinalIK.HandPoser>().poseRoot = target.transform;
		//}


		HoldenItem _holdenItem = interactionObject.GetComponent<HoldenItem>();

		if (_holdenItem == null)
		  _holdenItem = interactionObject.GetComponentInChildren<HoldenItem>();

		if (_holdenItem == null)
		  _holdenItem = interactionObject.GetComponentInParent<HoldenItem>();

		if (_holdenItem != null)
		{

		  it.Game.Player.MotionControllers.Motions.HoldItem holdItemMotion = mMotionController.GetMotion<it.Game.Player.MotionControllers.Motions.HoldItem>();

		  holdItemMotion.SetItem(_holdenItem);

			 //_holdenItem.Hold();
		  return;
		}
		if (interactionObject.GetComponent<Game.Items.Inventary.InventaryItem>())
		  _isGetItem = true;

	 }
	 private void OnInteractionOnResume(RootMotion.FinalIK.FullBodyBipedEffector effectorType, Game.Player.Interactions.InteractionObject interactionObject)
	 {
	 }
	 private void OnInteractionEvent(RootMotion.FinalIK.FullBodyBipedEffector effectorType, Game.Player.Interactions.InteractionObject interactionObject, Game.Player.Interactions.InteractionObject.InteractionEvent interactionEvent)
	 {
	 }

	 public virtual IEnumerator MoveToTargetInternal(Interactions.InteractionObject rInteractionObject)
	 {
		if (!mIsMovingToTarget)
		{
		  ForceInteractionTarget = _playerBehaviour.InteractionSystem.GetClosestInteractionTargetInRange();
		  mIsMovingToTarget = true;

		  bool lStoredWalkRunMotionEnabled = false;
		  bool lStoredUseTransformPosition = false;
		  bool lStoredUseTransformRotation = false;

		  MotionController lMotionController = mMotionController;
		  ActorController lActorController = lMotionController._ActorController;

		  {
			 lStoredUseTransformPosition = lActorController.UseTransformPosition;
			 lStoredUseTransformRotation = lActorController.UseTransformRotation;
		  }

		  lActorController.UseTransformPosition = true;
		  lActorController.UseTransformRotation = true;

		  // Enable our strafing motion
		  MotionControllerMotion lWalkRunMotion = lMotionController.GetMotion(_WalkRunMotion, true);
		  if (lWalkRunMotion != null)
		  {
			 lStoredWalkRunMotionEnabled = lWalkRunMotion.IsEnabled;
			 lWalkRunMotion.IsEnabled = true;
		  }

		  Vector3 lTargetPosition = lActorController._Transform.position;
		  //if (rInteractableCore.ForcePosition)
		  //{
		  //if (rInteractableCore.TargetLocation != null)
		  //{
		  //lTargetPosition = rInteractableCore.TargetLocation.position;
		  //}
		  //else if (rInteractableCore.TargetDistance > 0f)
		  //{
		  Vector3 lInteractablePosition = rInteractionObject.gameObject.transform.position;
		  lInteractablePosition.y = lActorController._Transform.position.y;

		  if (rInteractionObject.OffsetUse == Interactions.InteractionObject.OffsetUseType.radius)
		  {

			 lTargetPosition = lInteractablePosition + ((lActorController._Transform.position - lInteractablePosition).normalized * rInteractionObject.DistanceUsePosition);
		  }else if(rInteractionObject.OffsetUse == Interactions.InteractionObject.OffsetUseType.forvard)
		  {
			 Vector3 forvard = rInteractionObject.transform.position - rInteractionObject.GetComponentInChildren<InteractionTrigger>().transform.position;
			 Quaternion lookForvard = Quaternion.LookRotation(-forvard, Vector3.up);
			 Quaternion lookPlayer = Quaternion.LookRotation((lActorController._Transform.position - rInteractionObject.gameObject.transform.position), Vector3.up);
			 //float angle = Quaternion.Angle(lookForvard, lookPlayer);
			 //float angle = Vector3.Angle(-forvard, mActorController._Transform.position - rInteractionObject.transform.position);
			 float angle = Vector3Ext.HorizontalAngleTo(-forvard, lActorController._Transform.position - lInteractablePosition);
			 float angleRad = ((Mathf.PI * angle) / 180);
			 
			 float height = Mathf.Sin(angleRad) * (mActorController._Transform.position - rInteractionObject.transform.position).magnitude;

			 

			 Vector3 tDist =  rInteractionObject.transform.position - forvard * rInteractionObject.DistanceUsePosition;
			 Quaternion qt = Quaternion.LookRotation(forvard, Vector3.up);
			 Vector3 pos1 = tDist + (qt * new Vector3(height, 0, 0));
			 Vector3 pos2 = tDist + (qt * new Vector3(-height, 0, 0));

			 if((lActorController._Transform.position - pos1).magnitude < (lActorController._Transform.position - pos2).magnitude)
				lTargetPosition = pos1;
			 else
				lTargetPosition = pos2;

			 //lTargetPosition = tDist + (qt * new Vector3(height, 0, 0));
		  }
		  //}
		  //}

		  Vector3 lTargetForward = lActorController._Transform.forward;
		  //if (rInteractableCore.ForceRotation)
		  //{
		  //if (rInteractableCore.TargetLocation != null)
		  //{
		  //lTargetForward = rInteractableCore.TargetLocation.forward;
		  //}
		  //else
		  //{
		  //Vector3 lInteractablePosition = rInteractionObject.gameObject.transform.position;
		  lInteractablePosition.y = lActorController._Transform.position.y;

		  lTargetForward = (lInteractablePosition - lActorController._Transform.position).normalized;
		  //}
		  //}

		  // Move to the target position and rotation
		  Vector3 lDirection = lTargetPosition - lActorController._Transform.position;
		  if (rInteractionObject.OffsetUse == Interactions.InteractionObject.OffsetUseType.radius)
		  {
			 lDirection = lTargetPosition - lActorController._Transform.position;
		  }
		  else if (rInteractionObject.OffsetUse == Interactions.InteractionObject.OffsetUseType.forvard)
		  {
			 Vector3 forvard = rInteractionObject.transform.position - rInteractionObject.GetComponentInChildren<InteractionTrigger>().transform.position;
			 lDirection = -forvard;
		  }

			 float lAngle = Vector3Ext.HorizontalAngleTo(lActorController._Transform.forward, lTargetForward);

		  bool needMove = HorizontalDistance(lActorController._Transform.position, lTargetPosition) >= 0.15f;
		  bool needRotate = Mathf.Abs(lAngle) > 0.2f;

		  if (needMove || needRotate)
		  {

			 while (HorizontalDistance(lActorController._Transform.position, lTargetPosition) > 0.01f || Mathf.Abs(lAngle) > 0.1f)
			 {
				float lDistance = Mathf.Min(lDirection.magnitude, _WalkSpeed * Time.deltaTime);
				mMotionController.ForcedInput = Vector2.down;
				//ForcedInput
				lActorController._Transform.position = lActorController._Transform.position + (lDirection.normalized * lDistance);

				float lYaw = Mathf.Sign(lAngle) * Mathf.Min(Mathf.Abs(lAngle), _RotationSpeed * Time.deltaTime);
				lActorController._Transform.rotation = (lActorController._Transform.rotation * Quaternion.Euler(0f, lYaw, 0f));

				yield return new WaitForEndOfFrame();
				lDirection = lTargetPosition - lActorController._Transform.position;
				lAngle = Vector3Ext.HorizontalAngleTo(lActorController._Transform.forward, lTargetForward);
			 }
		  }

		  // Reset the motion and movement options
		  if (lWalkRunMotion != null)
		  {
			 lWalkRunMotion.Deactivate();
			 lWalkRunMotion.IsEnabled = lStoredWalkRunMotionEnabled;
		  }
		  // Activate BasicInteraction
		  //mActiveForm = rInteractableCore.Form;
		  //InteractableCore = rInteractableCore;
		  ForceInteraction = rInteractionObject;
		  lMotionController.ActivateMotion(this);

		  // Give some final frames to get to the exact position
		  yield return new WaitForSeconds(0.2f);


		  lActorController.UseTransformPosition = lStoredUseTransformPosition;
		  lActorController.UseTransformRotation = lStoredUseTransformRotation;
		  mIsMovingToTarget = false;

		  lMotionController.TargetNormalizedSpeed = 1f;
		}
	 }

	 protected float HorizontalDistance(Vector3 rVector1, Vector3 rVector2)
	 {
		rVector2.y = rVector1.y;
		return Vector3.Distance(rVector1, rVector2);
	 }

	 public void DropHoldItemIfExists()
	 {
		if (_holdenItemMotion.IsHold)
		  _holdenItemMotion.DropItem();

		//if (_holdenItem != null)
		//{


		//  if (_holdenItem != null)
		//  {
		//	 Animator[] animators = mMotionController.GetComponentsInChildren<Animator>();
		//	 for (int i = 0; i < animators.Length; i++)
		//	 {
		//		animators[i].SetInteger("HoldItem", 0);
		//	 }
		//  }
		//  Debug.Log("DropHoldItemIfExists");
		//  _holdenItem.transform.SetParent(null);
		//  _holdenItem.Drop();
		//  _playerBehaviour.InteractionSystem.ResumeAll();
		//  _holdenItem = null;
		//}
	 }

	 private void RemoveItem()
	 {

		_isGetItem = false;
		var poser = (_item as Component).transform.GetComponent<RootMotion.FinalIK.Poser>();
		if (poser != null)
		{
		  poser.poseRoot = null;
		  poser.weight = 0f;
		}
		(_item as Component).transform.parent = null;
	 }

	 private void OnPreFBBIK()
	 {
	 }

	 private void OnPostFBBIK()
	 {
	 }

	 private void OnFixTransforms()
	 {
	 }

#if UNITY_EDITOR

	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;


		if (EditorHelper.IntField("Form", "Sets the LXMotionForm animator property to determine the animation for the motion. If value is < 0, we use the Actor Core's 'Default Form' state.", Form, mMotionController))
		{
		  lIsDirty = true;
		  Form = EditorHelper.FieldIntValue;
		}

		int lNewLayers1 = EditorHelper.LayerMaskField(new GUIContent("Item Layers", "Layers that identies objects that can be interacted with."), ItemLayer);
		if (lNewLayers1 != ItemLayer)
		{
		  lIsDirty = true;
		  ItemLayer = lNewLayers1;
		}

		if (EditorHelper.TextField("Action Alias", "Action alias that starts the action and then exits the action (mostly for debugging).", ActionAlias, mMotionController))
		{
		  lIsDirty = true;
		  ActionAlias = EditorHelper.FieldStringValue;
		}

		if (EditorHelper.IntField("Form", "Sets the LXMotionForm animator property to determine the animation for the motion.", Form, mMotionController))
		{
		  lIsDirty = true;
		  Form = EditorHelper.FieldIntValue;
		}

		if (EditorHelper.TextField("Walk Motion", "Walk motion to enable for any position shifting that we need to do.", WalkRunMotion, mMotionController))
		{
		  lIsDirty = true;
		  WalkRunMotion = EditorHelper.FieldStringValue;
		}

		GUILayout.Space(5f);

		if (EditorHelper.BoolField("Use Raycast", "Determines if we'll constantly shoot a ray to test for interactables.", IsInteractableRaycastEnabled, mMotionController))
		{
		  lIsDirty = true;
		  IsInteractableRaycastEnabled = EditorHelper.FieldBoolValue;
		}

		if (IsInteractableRaycastEnabled)
		{
		  int lNewLayers = EditorHelper.LayerMaskField(new GUIContent("Layers", "Layers that identies objects that can be interacted with."), InteractableLayers);
		  if (lNewLayers != InteractableLayers)
		  {
			 lIsDirty = true;
			 InteractableLayers = lNewLayers;
		  }
		}

		return lIsDirty;
	 }

#endif
	 #region Auto-Generated
	 // ************************************ START AUTO GENERATED ************************************

	 /// <summary>
	 /// These declarations go inside the class so you can test for which state
	 /// and transitions are active. Testing hash values is much faster than strings.
	 /// </summary>
	 public int STATE_Start = -1;
	 public int STATE_Idle_GrabHighFront = -1;
	 public int STATE_Idle_PickUp = -1;
	 public int STATE_Idle_PushButton = -1;
	 public int STATE_IdlePose = -1;
	 public int STATE_Idle = -1;
	 public int STATE_InteractionHight = -1;
	 public int STATE_InteractionLow = -1;
	 public int STATE_InteractionMiddle = -1;
	 public int TRANS_AnyState_Idle_PushButton = -1;
	 public int TRANS_EntryState_Idle_PushButton = -1;
	 public int TRANS_AnyState_Idle_GrabHighFront = -1;
	 public int TRANS_EntryState_Idle_GrabHighFront = -1;
	 public int TRANS_AnyState_Idle_PickUp = -1;
	 public int TRANS_EntryState_Idle_PickUp = -1;
	 public int TRANS_AnyState_Idle = -1;
	 public int TRANS_EntryState_Idle = -1;
	 public int TRANS_AnyState_InteractionMiddle = -1;
	 public int TRANS_EntryState_InteractionMiddle = -1;
	 public int TRANS_AnyState_InteractionLow = -1;
	 public int TRANS_EntryState_InteractionLow = -1;
	 public int TRANS_AnyState_InteractionHight = -1;
	 public int TRANS_EntryState_InteractionHight = -1;
	 public int TRANS_Idle_GrabHighFront_IdlePose = -1;
	 public int TRANS_Idle_PickUp_IdlePose = -1;
	 public int TRANS_Idle_PushButton_IdlePose = -1;
	 public int TRANS_Idle_IdlePose = -1;
	 public int TRANS_InteractionHight_IdlePose = -1;
	 public int TRANS_InteractionLow_IdlePose = -1;
	 public int TRANS_InteractionMiddle_IdlePose = -1;

	 /// <summary>
	 /// Determines if we're using auto-generated code
	 /// </summary>
	 public override bool HasAutoGeneratedCode
	 {
		get { return true; }
	 }

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsInMotionState
	 {
		get
		{
		  int lStateID = mMotionLayer._AnimatorStateID;
		  int lTransitionID = mMotionLayer._AnimatorTransitionID;

		  if (lTransitionID == 0)
		  {
			 if (lStateID == STATE_Start) { return true; }
			 if (lStateID == STATE_Idle_GrabHighFront) { return true; }
			 if (lStateID == STATE_Idle_PickUp) { return true; }
			 if (lStateID == STATE_Idle_PushButton) { return true; }
			 //if (lStateID == STATE_IdlePose) { return true; }
			 if (lStateID == STATE_Idle) { return true; }
			 if (lStateID == STATE_InteractionHight) { return true; }
			 if (lStateID == STATE_InteractionLow) { return true; }
			 if (lStateID == STATE_InteractionMiddle) { return true; }
		  }

		  if (lTransitionID == TRANS_AnyState_Idle_PushButton) { return true; }
		  if (lTransitionID == TRANS_EntryState_Idle_PushButton) { return true; }
		  if (lTransitionID == TRANS_AnyState_Idle_GrabHighFront) { return true; }
		  if (lTransitionID == TRANS_EntryState_Idle_GrabHighFront) { return true; }
		  if (lTransitionID == TRANS_AnyState_Idle_PickUp) { return true; }
		  if (lTransitionID == TRANS_EntryState_Idle_PickUp) { return true; }
		  if (lTransitionID == TRANS_AnyState_Idle) { return true; }
		  if (lTransitionID == TRANS_EntryState_Idle) { return true; }
		  if (lTransitionID == TRANS_AnyState_InteractionMiddle) { return true; }
		  if (lTransitionID == TRANS_EntryState_InteractionMiddle) { return true; }
		  if (lTransitionID == TRANS_AnyState_InteractionLow) { return true; }
		  if (lTransitionID == TRANS_EntryState_InteractionLow) { return true; }
		  if (lTransitionID == TRANS_AnyState_InteractionHight) { return true; }
		  if (lTransitionID == TRANS_EntryState_InteractionHight) { return true; }
		  if (lTransitionID == TRANS_Idle_GrabHighFront_IdlePose) { return true; }
		  if (lTransitionID == TRANS_Idle_PickUp_IdlePose) { return true; }
		  if (lTransitionID == TRANS_Idle_PushButton_IdlePose) { return true; }
		  if (lTransitionID == TRANS_Idle_IdlePose) { return true; }
		  if (lTransitionID == TRANS_InteractionHight_IdlePose) { return true; }
		  if (lTransitionID == TRANS_InteractionLow_IdlePose) { return true; }
		  if (lTransitionID == TRANS_InteractionMiddle_IdlePose) { return true; }
		  return false;
		}
	 }

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsMotionState(int rStateID)
	 {
		if (rStateID == STATE_Start) { return true; }
		if (rStateID == STATE_Idle_GrabHighFront) { return true; }
		if (rStateID == STATE_Idle_PickUp) { return true; }
		if (rStateID == STATE_Idle_PushButton) { return true; }
		if (rStateID == STATE_IdlePose) { return true; }
		if (rStateID == STATE_Idle) { return true; }
		if (rStateID == STATE_InteractionHight) { return true; }
		if (rStateID == STATE_InteractionLow) { return true; }
		if (rStateID == STATE_InteractionMiddle) { return true; }
		return false;
	 }

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsMotionState(int rStateID, int rTransitionID)
	 {
		if (rTransitionID == 0)
		{
		  if (rStateID == STATE_Start) { return true; }
		  if (rStateID == STATE_Idle_GrabHighFront) { return true; }
		  if (rStateID == STATE_Idle_PickUp) { return true; }
		  if (rStateID == STATE_Idle_PushButton) { return true; }
		  if (rStateID == STATE_IdlePose) { return true; }
		  if (rStateID == STATE_Idle) { return true; }
		  if (rStateID == STATE_InteractionHight) { return true; }
		  if (rStateID == STATE_InteractionLow) { return true; }
		  if (rStateID == STATE_InteractionMiddle) { return true; }
		}

		if (rTransitionID == TRANS_AnyState_Idle_PushButton) { return true; }
		if (rTransitionID == TRANS_EntryState_Idle_PushButton) { return true; }
		if (rTransitionID == TRANS_AnyState_Idle_GrabHighFront) { return true; }
		if (rTransitionID == TRANS_EntryState_Idle_GrabHighFront) { return true; }
		if (rTransitionID == TRANS_AnyState_Idle_PickUp) { return true; }
		if (rTransitionID == TRANS_EntryState_Idle_PickUp) { return true; }
		if (rTransitionID == TRANS_AnyState_Idle) { return true; }
		if (rTransitionID == TRANS_EntryState_Idle) { return true; }
		if (rTransitionID == TRANS_AnyState_InteractionMiddle) { return true; }
		if (rTransitionID == TRANS_EntryState_InteractionMiddle) { return true; }
		if (rTransitionID == TRANS_AnyState_InteractionLow) { return true; }
		if (rTransitionID == TRANS_EntryState_InteractionLow) { return true; }
		if (rTransitionID == TRANS_AnyState_InteractionHight) { return true; }
		if (rTransitionID == TRANS_EntryState_InteractionHight) { return true; }
		if (rTransitionID == TRANS_Idle_GrabHighFront_IdlePose) { return true; }
		if (rTransitionID == TRANS_Idle_PickUp_IdlePose) { return true; }
		if (rTransitionID == TRANS_Idle_PushButton_IdlePose) { return true; }
		if (rTransitionID == TRANS_Idle_IdlePose) { return true; }
		if (rTransitionID == TRANS_InteractionHight_IdlePose) { return true; }
		if (rTransitionID == TRANS_InteractionLow_IdlePose) { return true; }
		if (rTransitionID == TRANS_InteractionMiddle_IdlePose) { return true; }
		return false;
	 }

	 /// <summary>
	 /// Preprocess any animator data so the motion can use it later
	 /// </summary>
	 public override void LoadAnimatorData()
	 {
		string lLayer = mMotionController.Animator.GetLayerName(mMotionLayer._AnimatorLayerIndex);
		TRANS_AnyState_Idle_PushButton = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Interaction-SM.Idle_PushButton");
		TRANS_EntryState_Idle_PushButton = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Interaction-SM.Idle_PushButton");
		TRANS_AnyState_Idle_GrabHighFront = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Interaction-SM.Idle_GrabHighFront");
		TRANS_EntryState_Idle_GrabHighFront = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Interaction-SM.Idle_GrabHighFront");
		TRANS_AnyState_Idle_PickUp = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Interaction-SM.Idle_PickUp");
		TRANS_EntryState_Idle_PickUp = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Interaction-SM.Idle_PickUp");
		TRANS_AnyState_Idle = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Interaction-SM.Idle");
		TRANS_EntryState_Idle = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Interaction-SM.Idle");
		TRANS_AnyState_InteractionMiddle = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Interaction-SM.InteractionMiddle");
		TRANS_EntryState_InteractionMiddle = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Interaction-SM.InteractionMiddle");
		TRANS_AnyState_InteractionLow = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Interaction-SM.InteractionLow");
		TRANS_EntryState_InteractionLow = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Interaction-SM.InteractionLow");
		TRANS_AnyState_InteractionHight = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Interaction-SM.InteractionHight");
		TRANS_EntryState_InteractionHight = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Interaction-SM.InteractionHight");
		STATE_Start = mMotionController.AddAnimatorName("" + lLayer + ".Start");
		STATE_Idle_GrabHighFront = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.Idle_GrabHighFront");
		TRANS_Idle_GrabHighFront_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.Idle_GrabHighFront -> " + lLayer + ".Interaction-SM.IdlePose");
		STATE_Idle_PickUp = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.Idle_PickUp");
		TRANS_Idle_PickUp_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.Idle_PickUp -> " + lLayer + ".Interaction-SM.IdlePose");
		STATE_Idle_PushButton = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.Idle_PushButton");
		TRANS_Idle_PushButton_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.Idle_PushButton -> " + lLayer + ".Interaction-SM.IdlePose");
		STATE_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.IdlePose");
		STATE_Idle = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.Idle");
		TRANS_Idle_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.Idle -> " + lLayer + ".Interaction-SM.IdlePose");
		STATE_InteractionHight = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.InteractionHight");
		TRANS_InteractionHight_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.InteractionHight -> " + lLayer + ".Interaction-SM.IdlePose");
		STATE_InteractionLow = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.InteractionLow");
		TRANS_InteractionLow_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.InteractionLow -> " + lLayer + ".Interaction-SM.IdlePose");
		STATE_InteractionMiddle = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.InteractionMiddle");
		TRANS_InteractionMiddle_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Interaction-SM.InteractionMiddle -> " + lLayer + ".Interaction-SM.IdlePose");
	 }

#if UNITY_EDITOR

	 /// <summary>
	 /// New way to create sub-state machines without destroying what exists first.
	 /// </summary>
	 protected override void CreateStateMachine()
	 {
		int rLayerIndex = mMotionLayer._AnimatorLayerIndex;
		MotionController rMotionController = mMotionController;

		UnityEditor.Animations.AnimatorController lController = null;

		Animator lAnimator = rMotionController.Animator;
		if (lAnimator == null) { lAnimator = rMotionController.gameObject.GetComponent<Animator>(); }
		if (lAnimator != null) { lController = lAnimator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController; }
		if (lController == null) { return; }

		while (lController.layers.Length <= rLayerIndex)
		{
		  UnityEditor.Animations.AnimatorControllerLayer lNewLayer = new UnityEditor.Animations.AnimatorControllerLayer();
		  lNewLayer.name = "Layer " + (lController.layers.Length + 1);
		  lNewLayer.stateMachine = new UnityEditor.Animations.AnimatorStateMachine();
		  lController.AddLayer(lNewLayer);
		}

		UnityEditor.Animations.AnimatorControllerLayer lLayer = lController.layers[rLayerIndex];

		UnityEditor.Animations.AnimatorStateMachine lLayerStateMachine = lLayer.stateMachine;

		UnityEditor.Animations.AnimatorStateMachine lSSM_295166 = MotionControllerMotion.EditorFindSSM(lLayerStateMachine, "Interaction-SM");
		if (lSSM_295166 == null) { lSSM_295166 = lLayerStateMachine.AddStateMachine("Interaction-SM", new Vector3(970, 300, 0)); }

		UnityEditor.Animations.AnimatorState lState_295232 = MotionControllerMotion.EditorFindState(lSSM_295166, "Idle_GrabHighFront");
		if (lState_295232 == null) { lState_295232 = lSSM_295166.AddState("Idle_GrabHighFront", new Vector3(330, -40, 0)); }
		lState_295232.speed = 1.5f;
		lState_295232.mirror = false;
		lState_295232.tag = "";
		lState_295232.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/Interactable.fbx", "Idle_GrabHighFront");

		UnityEditor.Animations.AnimatorState lState_295234 = MotionControllerMotion.EditorFindState(lSSM_295166, "Idle_PickUp");
		if (lState_295234 == null) { lState_295234 = lSSM_295166.AddState("Idle_PickUp", new Vector3(330, 80, 0)); }
		lState_295234.speed = 1.5f;
		lState_295234.mirror = false;
		lState_295234.tag = "";
		lState_295234.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/Interactable.fbx", "Idle_PickUp");

		UnityEditor.Animations.AnimatorState lState_295236 = MotionControllerMotion.EditorFindState(lSSM_295166, "Idle_PushButton");
		if (lState_295236 == null) { lState_295236 = lSSM_295166.AddState("Idle_PushButton", new Vector3(330, -140, 0)); }
		lState_295236.speed = 1.5f;
		lState_295236.mirror = false;
		lState_295236.tag = "";
		lState_295236.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/Interactable.fbx", "Idle_PushButton");

		UnityEditor.Animations.AnimatorState lState_295238 = MotionControllerMotion.EditorFindState(lSSM_295166, "IdlePose");
		if (lState_295238 == null) { lState_295238 = lSSM_295166.AddState("IdlePose", new Vector3(600, 48, 0)); }
		lState_295238.speed = 1f;
		lState_295238.mirror = false;
		lState_295238.tag = "Exit";
		lState_295238.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/IdleWalkRun.fbx", "Idle");

		UnityEditor.Animations.AnimatorState lState_295240 = MotionControllerMotion.EditorFindState(lSSM_295166, "Idle");
		if (lState_295240 == null) { lState_295240 = lSSM_295166.AddState("Idle", new Vector3(330, 160, 0)); }
		lState_295240.speed = 1f;
		lState_295240.mirror = false;
		lState_295240.tag = "";
		lState_295240.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/IdleWalkRun.fbx", "Idle");

		UnityEditor.Animations.AnimatorState lState_295242 = MotionControllerMotion.EditorFindState(lSSM_295166, "InteractionHight");
		if (lState_295242 == null) { lState_295242 = lSSM_295166.AddState("InteractionHight", new Vector3(540, 380, 0)); }
		lState_295242.speed = 1f;
		lState_295242.mirror = false;
		lState_295242.tag = "";
		lState_295242.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/Interaction.fbx", "InteractionHight");

		UnityEditor.Animations.AnimatorState lState_295244 = MotionControllerMotion.EditorFindState(lSSM_295166, "InteractionLow");
		if (lState_295244 == null) { lState_295244 = lSSM_295166.AddState("InteractionLow", new Vector3(540, 330, 0)); }
		lState_295244.speed = 1f;
		lState_295244.mirror = false;
		lState_295244.tag = "";
		lState_295244.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/Interaction.fbx", "InteractionLow");

		UnityEditor.Animations.AnimatorState lState_295246 = MotionControllerMotion.EditorFindState(lSSM_295166, "InteractionMiddle");
		if (lState_295246 == null) { lState_295246 = lSSM_295166.AddState("InteractionMiddle", new Vector3(530, 250, 0)); }
		lState_295246.speed = 1f;
		lState_295246.mirror = false;
		lState_295246.tag = "";
		lState_295246.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/Interaction.fbx", "InteractionMiddle");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_295416 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_295236, 0);
		if (lAnyTransition_295416 == null) { lAnyTransition_295416 = lLayerStateMachine.AddAnyStateTransition(lState_295236); }
		lAnyTransition_295416.isExit = false;
		lAnyTransition_295416.hasExitTime = false;
		lAnyTransition_295416.hasFixedDuration = true;
		lAnyTransition_295416.exitTime = 0.75f;
		lAnyTransition_295416.duration = 0.25f;
		lAnyTransition_295416.offset = 0.1517324f;
		lAnyTransition_295416.mute = false;
		lAnyTransition_295416.solo = false;
		lAnyTransition_295416.canTransitionToSelf = true;
		lAnyTransition_295416.orderedInterruption = true;
		lAnyTransition_295416.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_295416.conditions.Length - 1; i >= 0; i--) { lAnyTransition_295416.RemoveCondition(lAnyTransition_295416.conditions[i]); }
		lAnyTransition_295416.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 3450f, "L" + rLayerIndex + "MotionPhase");
		lAnyTransition_295416.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 0f, "L" + rLayerIndex + "MotionForm");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_295418 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_295232, 0);
		if (lAnyTransition_295418 == null) { lAnyTransition_295418 = lLayerStateMachine.AddAnyStateTransition(lState_295232); }
		lAnyTransition_295418.isExit = false;
		lAnyTransition_295418.hasExitTime = false;
		lAnyTransition_295418.hasFixedDuration = true;
		lAnyTransition_295418.exitTime = 0.75f;
		lAnyTransition_295418.duration = 0.25f;
		lAnyTransition_295418.offset = 0.07021895f;
		lAnyTransition_295418.mute = false;
		lAnyTransition_295418.solo = false;
		lAnyTransition_295418.canTransitionToSelf = true;
		lAnyTransition_295418.orderedInterruption = true;
		lAnyTransition_295418.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_295418.conditions.Length - 1; i >= 0; i--) { lAnyTransition_295418.RemoveCondition(lAnyTransition_295418.conditions[i]); }
		lAnyTransition_295418.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 3450f, "L" + rLayerIndex + "MotionPhase");
		lAnyTransition_295418.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 1f, "L" + rLayerIndex + "MotionForm");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_295420 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_295234, 0);
		if (lAnyTransition_295420 == null) { lAnyTransition_295420 = lLayerStateMachine.AddAnyStateTransition(lState_295234); }
		lAnyTransition_295420.isExit = false;
		lAnyTransition_295420.hasExitTime = false;
		lAnyTransition_295420.hasFixedDuration = true;
		lAnyTransition_295420.exitTime = 0.75f;
		lAnyTransition_295420.duration = 0.25f;
		lAnyTransition_295420.offset = 0f;
		lAnyTransition_295420.mute = false;
		lAnyTransition_295420.solo = false;
		lAnyTransition_295420.canTransitionToSelf = true;
		lAnyTransition_295420.orderedInterruption = true;
		lAnyTransition_295420.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_295420.conditions.Length - 1; i >= 0; i--) { lAnyTransition_295420.RemoveCondition(lAnyTransition_295420.conditions[i]); }
		lAnyTransition_295420.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 3450f, "L" + rLayerIndex + "MotionPhase");
		lAnyTransition_295420.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 2f, "L" + rLayerIndex + "MotionForm");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_295422 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_295240, 0);
		if (lAnyTransition_295422 == null) { lAnyTransition_295422 = lLayerStateMachine.AddAnyStateTransition(lState_295240); }
		lAnyTransition_295422.isExit = false;
		lAnyTransition_295422.hasExitTime = false;
		lAnyTransition_295422.hasFixedDuration = true;
		lAnyTransition_295422.exitTime = 0.75f;
		lAnyTransition_295422.duration = 0.25f;
		lAnyTransition_295422.offset = 0f;
		lAnyTransition_295422.mute = false;
		lAnyTransition_295422.solo = false;
		lAnyTransition_295422.canTransitionToSelf = true;
		lAnyTransition_295422.orderedInterruption = true;
		lAnyTransition_295422.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_295422.conditions.Length - 1; i >= 0; i--) { lAnyTransition_295422.RemoveCondition(lAnyTransition_295422.conditions[i]); }
		lAnyTransition_295422.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 3450f, "L" + rLayerIndex + "MotionPhase");
		lAnyTransition_295422.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 111f, "L" + rLayerIndex + "MotionForm");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_295446 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_295246, 0);
		if (lAnyTransition_295446 == null) { lAnyTransition_295446 = lLayerStateMachine.AddAnyStateTransition(lState_295246); }
		lAnyTransition_295446.isExit = false;
		lAnyTransition_295446.hasExitTime = false;
		lAnyTransition_295446.hasFixedDuration = true;
		lAnyTransition_295446.exitTime = 0.75f;
		lAnyTransition_295446.duration = 0.25f;
		lAnyTransition_295446.offset = 0f;
		lAnyTransition_295446.mute = false;
		lAnyTransition_295446.solo = false;
		lAnyTransition_295446.canTransitionToSelf = false;
		lAnyTransition_295446.orderedInterruption = true;
		lAnyTransition_295446.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_295446.conditions.Length - 1; i >= 0; i--) { lAnyTransition_295446.RemoveCondition(lAnyTransition_295446.conditions[i]); }
		lAnyTransition_295446.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 3452f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_295448 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_295244, 0);
		if (lAnyTransition_295448 == null) { lAnyTransition_295448 = lLayerStateMachine.AddAnyStateTransition(lState_295244); }
		lAnyTransition_295448.isExit = false;
		lAnyTransition_295448.hasExitTime = false;
		lAnyTransition_295448.hasFixedDuration = true;
		lAnyTransition_295448.exitTime = 0.75f;
		lAnyTransition_295448.duration = 0.25f;
		lAnyTransition_295448.offset = 0f;
		lAnyTransition_295448.mute = false;
		lAnyTransition_295448.solo = false;
		lAnyTransition_295448.canTransitionToSelf = false;
		lAnyTransition_295448.orderedInterruption = true;
		lAnyTransition_295448.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_295448.conditions.Length - 1; i >= 0; i--) { lAnyTransition_295448.RemoveCondition(lAnyTransition_295448.conditions[i]); }
		lAnyTransition_295448.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 3451f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_295450 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_295242, 0);
		if (lAnyTransition_295450 == null) { lAnyTransition_295450 = lLayerStateMachine.AddAnyStateTransition(lState_295242); }
		lAnyTransition_295450.isExit = false;
		lAnyTransition_295450.hasExitTime = false;
		lAnyTransition_295450.hasFixedDuration = true;
		lAnyTransition_295450.exitTime = 0.75f;
		lAnyTransition_295450.duration = 0.25f;
		lAnyTransition_295450.offset = 0f;
		lAnyTransition_295450.mute = false;
		lAnyTransition_295450.solo = false;
		lAnyTransition_295450.canTransitionToSelf = false;
		lAnyTransition_295450.orderedInterruption = true;
		lAnyTransition_295450.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_295450.conditions.Length - 1; i >= 0; i--) { lAnyTransition_295450.RemoveCondition(lAnyTransition_295450.conditions[i]); }
		lAnyTransition_295450.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 3453f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_296160 = MotionControllerMotion.EditorFindTransition(lState_295232, lState_295238, 0);
		if (lTransition_296160 == null) { lTransition_296160 = lState_295232.AddTransition(lState_295238); }
		lTransition_296160.isExit = false;
		lTransition_296160.hasExitTime = true;
		lTransition_296160.hasFixedDuration = true;
		lTransition_296160.exitTime = 0.9285715f;
		lTransition_296160.duration = 0.25f;
		lTransition_296160.offset = 0f;
		lTransition_296160.mute = false;
		lTransition_296160.solo = false;
		lTransition_296160.canTransitionToSelf = true;
		lTransition_296160.orderedInterruption = true;
		lTransition_296160.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_296160.conditions.Length - 1; i >= 0; i--) { lTransition_296160.RemoveCondition(lTransition_296160.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_296164 = MotionControllerMotion.EditorFindTransition(lState_295234, lState_295238, 0);
		if (lTransition_296164 == null) { lTransition_296164 = lState_295234.AddTransition(lState_295238); }
		lTransition_296164.isExit = false;
		lTransition_296164.hasExitTime = true;
		lTransition_296164.hasFixedDuration = true;
		lTransition_296164.exitTime = 0.90625f;
		lTransition_296164.duration = 0.25f;
		lTransition_296164.offset = 0f;
		lTransition_296164.mute = false;
		lTransition_296164.solo = false;
		lTransition_296164.canTransitionToSelf = true;
		lTransition_296164.orderedInterruption = true;
		lTransition_296164.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_296164.conditions.Length - 1; i >= 0; i--) { lTransition_296164.RemoveCondition(lTransition_296164.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_296168 = MotionControllerMotion.EditorFindTransition(lState_295236, lState_295238, 0);
		if (lTransition_296168 == null) { lTransition_296168 = lState_295236.AddTransition(lState_295238); }
		lTransition_296168.isExit = false;
		lTransition_296168.hasExitTime = true;
		lTransition_296168.hasFixedDuration = true;
		lTransition_296168.exitTime = 0.6807526f;
		lTransition_296168.duration = 0.1987377f;
		lTransition_296168.offset = 0f;
		lTransition_296168.mute = false;
		lTransition_296168.solo = false;
		lTransition_296168.canTransitionToSelf = true;
		lTransition_296168.orderedInterruption = true;
		lTransition_296168.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_296168.conditions.Length - 1; i >= 0; i--) { lTransition_296168.RemoveCondition(lTransition_296168.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_296172 = MotionControllerMotion.EditorFindTransition(lState_295240, lState_295238, 0);
		if (lTransition_296172 == null) { lTransition_296172 = lState_295240.AddTransition(lState_295238); }
		lTransition_296172.isExit = false;
		lTransition_296172.hasExitTime = true;
		lTransition_296172.hasFixedDuration = true;
		lTransition_296172.exitTime = 0.4087157f;
		lTransition_296172.duration = 0.25f;
		lTransition_296172.offset = 0f;
		lTransition_296172.mute = false;
		lTransition_296172.solo = false;
		lTransition_296172.canTransitionToSelf = true;
		lTransition_296172.orderedInterruption = true;
		lTransition_296172.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_296172.conditions.Length - 1; i >= 0; i--) { lTransition_296172.RemoveCondition(lTransition_296172.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_296174 = MotionControllerMotion.EditorFindTransition(lState_295242, lState_295238, 0);
		if (lTransition_296174 == null) { lTransition_296174 = lState_295242.AddTransition(lState_295238); }
		lTransition_296174.isExit = false;
		lTransition_296174.hasExitTime = true;
		lTransition_296174.hasFixedDuration = true;
		lTransition_296174.exitTime = 0.9305556f;
		lTransition_296174.duration = 0.25f;
		lTransition_296174.offset = 0f;
		lTransition_296174.mute = false;
		lTransition_296174.solo = false;
		lTransition_296174.canTransitionToSelf = true;
		lTransition_296174.orderedInterruption = true;
		lTransition_296174.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_296174.conditions.Length - 1; i >= 0; i--) { lTransition_296174.RemoveCondition(lTransition_296174.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_296178 = MotionControllerMotion.EditorFindTransition(lState_295244, lState_295238, 0);
		if (lTransition_296178 == null) { lTransition_296178 = lState_295244.AddTransition(lState_295238); }
		lTransition_296178.isExit = false;
		lTransition_296178.hasExitTime = true;
		lTransition_296178.hasFixedDuration = true;
		lTransition_296178.exitTime = 0.9147727f;
		lTransition_296178.duration = 0.25f;
		lTransition_296178.offset = 0f;
		lTransition_296178.mute = false;
		lTransition_296178.solo = false;
		lTransition_296178.canTransitionToSelf = true;
		lTransition_296178.orderedInterruption = true;
		lTransition_296178.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_296178.conditions.Length - 1; i >= 0; i--) { lTransition_296178.RemoveCondition(lTransition_296178.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_296182 = MotionControllerMotion.EditorFindTransition(lState_295246, lState_295238, 0);
		if (lTransition_296182 == null) { lTransition_296182 = lState_295246.AddTransition(lState_295238); }
		lTransition_296182.isExit = false;
		lTransition_296182.hasExitTime = true;
		lTransition_296182.hasFixedDuration = true;
		lTransition_296182.exitTime = 0.8880597f;
		lTransition_296182.duration = 0.25f;
		lTransition_296182.offset = 0f;
		lTransition_296182.mute = false;
		lTransition_296182.solo = false;
		lTransition_296182.canTransitionToSelf = true;
		lTransition_296182.orderedInterruption = true;
		lTransition_296182.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_296182.conditions.Length - 1; i >= 0; i--) { lTransition_296182.RemoveCondition(lTransition_296182.conditions[i]); }


		// Run any post processing after creating the state machine
		OnStateMachineCreated();
	 }

#endif

	 // ************************************ END AUTO GENERATED ************************************
	 #endregion

  }
}