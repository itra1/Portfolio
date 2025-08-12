using System;
using System.Collections;
using System.Collections.Generic;

using com.ootii.Cameras;
using com.ootii.Geometry;

using it.Game.Managers;
using UnityEngine;


namespace it.Game.Environment.Level5.TrapRoom
{
  public class TrapRoom : Challenge
  {
	 [SerializeField]
	 private Piramida[] _piramides;
	 [SerializeField]
	 private Handlers.Lighting[] _roundLighting;
	 [SerializeField]
	 private Handlers.Lighting[] _centerLighting;

	 [SerializeField]
	 private Transform _centerRotate;
	 [SerializeField]
	 private Transform _cameraForvardRotate;

	 [SerializeField]
	 private Transform _rayStart;

	 private int step = 0;

	 [SerializeField]
	 private Transform _cameraTransform;
	 [SerializeField]
	 private LayerMask _layers;

	 private Piramida _oldPiramida;

	 protected override void Start()
	 {
		base.Start();
		DeactivateAll();
	 }

	 public void ActivateLight()
	 {
		step++;
		switch (step)
		{
		  case 1:
			 LightingActivate(_roundLighting,true);
			 break;
		  case 2:
			 LightingActivate(_centerLighting, true);
			 break;
		  case 3:
			 LightingActivate(_centerLighting, false);
			 break;
		}
	 }

	 private void DeactivateAll()
	 {
		for(int i = 0; i < _piramides.Length; i++)
		{
		  _piramides[i].SetActiveState(false);
		}
	 }

	 private void LightingActivate(Game.Environment.Handlers.Lighting[] array, bool active)
	 {
		for(int i = 0; i < array.Length; i++)
		{
		  if (active)
			 array[i].StartVisualLine();
		  else
			 array[i].Stop();
		}
	 }

	 public override void SetActivate()
	 {
		base.SetActivate();
		State = 1;
		SetCameraFixedPosition();
		sLerpY = 0;
		DeactivateAll();
		_oldPiramida = null;
		for (int i = 0; i < _piramides.Length; i++)
		{
		  _piramides[i].ActivateSymbol();
		}

	 }

	 public override void SetDeactivate()
	 {
		base.SetDeactivate();

		if(State == 2)
		{

		}
		else
		{

		}

	 }

	 private float sLerpY;

	 public override bool IsInteractReady => true;

	 private void Update()
	 {
		if (State != 1)
		  return;

		float moveX = GameManager.Instance.EnvironmentInputSource.MovementX;
		float moveY = GameManager.Instance.EnvironmentInputSource.MovementY;

		sLerpY += moveY * Time.deltaTime * 0.3f;

		sLerpY = Mathf.Clamp(sLerpY, 0, 1);

		_centerRotate.rotation *= Quaternion.Euler(0, moveX*0.1f, 0);

		//_cameraForvardRotate.rotation *= Quaternion.Euler(-moveY, 0, 0);
		_cameraForvardRotate.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(20, 0, 0), sLerpY);
		RayCastCheck();
	 }

	 private void RayCastCheck()
	 {
		RaycastHit _rayHit;
		if(RaycastExt.SafeRaycast(_rayStart.position, _rayStart.forward,out _rayHit,20, _layers, null))
		{
		  Piramida piramida = _rayHit.transform.GetComponentInParent<Piramida>();

		  if(piramida != null)
		  {
			 if (_oldPiramida == piramida)
				return;

			 _oldPiramida = piramida;

			 if (!piramida.IsCurrect)
			 {
				// Урон игроку
			 }
			 else
			 {
				piramida.SetActiveState(true);
				CheckComplete();
			 }
		  }
		}
	 }

	 private void CheckComplete()
	 {
		for(int i = 0; i < _piramides.Length; i++)
		{
		  if (_piramides[i].IsCurrect && !_piramides[i].IsActive)
			 return;
		}

		Complete();
		StopInteract();
	}

	 private void SetCameraFixedPosition()
	 {

		var fixedMotor = CameraBehaviour.Instance.CameraController.GetMotor<CopyTransformFromTarget>("FixedPosition");
		fixedMotor.Target = _cameraTransform;
		CameraBehaviour.Instance.TransitionMotor(fixedMotor,1);
	 }

  }
}