using UnityEngine;
using System.Collections.Generic;
using com.ootii.Messages;
using it.Game.Player.Save;
using Leguar.TotalJSON;
using it.Game.Handles;
using it.Game.Managers;
using it.Game.Player.Interactions;

namespace it.Game.Environment
{
  public abstract class Challenge : Environment, Game.Items.IInteraction, IEscape, IInteractionCondition
  {
	 /*
	  * Стандартные состояния:
	  * -1 - не активно
	  * 0 - готов к использованию
	  * 1 - используется
	  * 2 - победа
	  * 3 - поражение
	  * 
	  */

	 public UnityEngine.Events.UnityEvent OnComplete;
	 public UnityEngine.Events.UnityEvent OnFailed;

	 [SerializeField]
	 protected bool _isEscapeDeactivate = false;
	 [SerializeField]
	 protected bool _isDeacivateGameInput = false;
	 [SerializeField]
	 protected bool _isEnableEnvironmentInput = false;
	 [SerializeField]
	 protected bool _isHidePlayer = false;
	 [SerializeField]
	 protected bool _isMouseEnable = false;
	 protected virtual int CompleteState => 2;

	 private bool _isActive;
	 public bool IsActive { get { return State == 1; } }

	 public virtual bool IsComplete { get => State == 2; }

	 public abstract bool IsInteractReady { get; }
	 public virtual bool InteractionReady()
	 {
		return true;
	 }
	 protected MouseHandler _mouseHandler;

	 /// <summary>
	 /// Activation
	 /// </summary>
	 public virtual void SetActivate() { }
	 public virtual void SetDeactivate() {	 }
	 public virtual void Escape()
	 {
		StopInteract();
	 }

	 protected virtual void SetColliderActivate(bool isActive) { }

	 protected override void Start()
	 {
		base.Start();
		DeInitialization();
	 }

	 protected void AddMouseHandler()
	 {
		_mouseHandler = GetComponent<MouseHandler>();
		if (_mouseHandler == null)
		  _mouseHandler = gameObject.AddComponent<MouseHandler>();
	 }

	 protected void RemoveMouseHandler()
	 {
		if (_mouseHandler != null)
		  Destroy(_mouseHandler);
	 }

	 public virtual void FocusCamera(Transform target, float time)
	 {
		var camera = CameraBehaviour.Instance.gameObject.GetComponent<com.ootii.Cameras.CameraController>();
		var cameraTargetMotion = camera.GetMotor<com.ootii.Cameras.CopyTransformFromTarget>();
		var transitor = camera.GetMotor<com.ootii.Cameras.TransitionMotor>("0 to 1");
		transitor.TransitionTime = time;
		cameraTargetMotion.Target = target;
		camera.ActivateMotor(2);
	 }
	 public virtual void UnFocusCamera(float time)
	 {
		var camera = CameraBehaviour.Instance.gameObject.GetComponent<com.ootii.Cameras.CameraController>();
		var cameraTargetMotion = camera.GetMotor<com.ootii.Cameras.CopyTransformFromTarget>();
		var transitor = camera.GetMotor<com.ootii.Cameras.TransitionMotor>("1 to 0");
		transitor.TransitionTime = time;

		camera.ActivateMotor(3);
	 }

	 protected virtual void Complete()
	 {
		if (IsComplete)
		  return;

		State = CompleteState;
		ConfirmState();
		EmitComplete();
		Save();
	 }


	 public virtual void EmitComplete()
	 {
		OnComplete?.Invoke();
	 }
	 public virtual void EmitFailed()
	 {
		OnFailed?.Invoke();
	 }

	 protected virtual void Failed()
	 {
		EmitFailed();
	 }

	 public virtual void StartInteract() {

		SetActivate();
		Initialization();
	 }

	 public virtual void StopInteract()
	 {
		SetDeactivate();
		DeInitialization();
	 }
	 protected virtual void Initialization() {

		AddMouseHandler();
		SetColliderActivate(true);
		if (_isEscapeDeactivate)
		  GameManager.Instance.AddEscape(this);
		if (_isDeacivateGameInput)
		  GameManager.Instance.GameInputSource.enabled = false;
		if (_isEnableEnvironmentInput)
		  GameManager.Instance.EnvironmentInputSource.enabled = true;
		if (_isHidePlayer)
		  CameraBehaviour.Instance.HidePlayer(true);
		if (_isMouseEnable)
		  GameManager.Instance.SetCursorVisible(this, true);

	 }

	 protected virtual void DeInitialization() {

		RemoveMouseHandler();
		SetColliderActivate(false);
		if (_isEscapeDeactivate)
		  GameManager.Instance.RemoveEscape(this);
		if (_isDeacivateGameInput)
		  GameManager.Instance.GameInputSource.enabled = true;
		if (_isEnableEnvironmentInput)
		  GameManager.Instance.EnvironmentInputSource.enabled = false;
		if (_isHidePlayer)
		  CameraBehaviour.Instance.HidePlayer(false);
		if (_isMouseEnable)
		  GameManager.Instance.SetCursorVisible(this, false);
	 }

  }
}