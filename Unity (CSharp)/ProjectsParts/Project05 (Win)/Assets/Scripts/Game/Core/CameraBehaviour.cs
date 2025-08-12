using System.Linq;
using System.Collections;
using System.Collections.Generic;
using com.ootii.Cameras;
using Aura2API;

using JetBrains.Annotations;

using Obi;

using UnityEngine;
using it.Game.Player;

public class CameraBehaviour : Singleton<CameraBehaviour>
{
  [SerializeField]
  private LayerMask _standartCulling;
  [SerializeField]
  private LayerMask _cullingNoPlayer;

  private ObiFluidRenderer _fluedRenderer;
  private ObiFluidRenderer FluedRenderer
  {
	 get
	 {
		if (_fluedRenderer == null)
		  _fluedRenderer = GetComponentInChildren<ObiFluidRenderer>();
		return _fluedRenderer;
	 }
  }


  public Camera Camera
  {
	 get
	 {
		if (_camera == null)
		  _camera = GetComponentInChildren<Camera>();
		return _camera;
	 }
	 set => _camera = value;
  }

  public AuraCamera AuraCamera
  {
	 get
	 {
		if (_auraCamera == null)
		  _auraCamera = GetComponentInChildren<Aura2API.AuraCamera>();
		return _auraCamera;
	 }
	 set => _auraCamera = value;
  }

  private Camera _camera;

  private com.ootii.Cameras.CameraController _cameraController;

  public com.ootii.Cameras.CameraController CameraController => _cameraController ?? (_cameraController = GetComponent<com.ootii.Cameras.CameraController>());

  private bool _playerFollow = true;

  private Aura2API.AuraCamera _auraCamera;



  private void OnEnable()
  {
	 var variablePlayMaker = HutongGames.PlayMaker.FsmVariables.GlobalVariables.GetFsmGameObject("Camera");
	 variablePlayMaker.Value = gameObject;
  }

  /// <summary>
  /// Следовать за играком
  /// </summary>
  [SerializeField]
  public bool PlayerFollow
  {
	 get
	 {
		return _playerFollow;
	 }
	 set
	 {
		if (_playerFollow == value)
		  return;
		_playerFollow = value;
		//ConfirmChangePlayerFollow();
	 }
  }

  public void AddFluedRendere(ObiParticleRenderer renderer)
  {
	 bool isExists = false;
	 for (int i = 0; i < FluedRenderer.particleRenderers.Length; i++)
	 {
		if (FluedRenderer.particleRenderers[i] == renderer)
		  isExists = true;
	 }
	 if (!isExists)
	 {
		var list = FluedRenderer.particleRenderers.ToList();
		list.Add(renderer);
		FluedRenderer.particleRenderers = list.ToArray();
	 }
  }
  public void RemoveFluedRendere(ObiParticleRenderer renderer)
  {
	 var list = FluedRenderer.particleRenderers.ToList();
	 list.Remove(renderer);
	 FluedRenderer.particleRenderers = list.ToArray();
  }


  public void HidePlayer(bool isHide)
  {
	 Camera.cullingMask = isHide ? _cullingNoPlayer : _standartCulling;
	 if (PlayerBehaviour.Instance != null)
		PlayerBehaviour.Instance.HidePlayerCamera(isHide);
  }

  public void ActivateMotor(string nameMotor)
  {
	 var motor = _cameraController.GetMotor(nameMotor);
	 ActivateMotor(motor);
  }
  public void ActivateMotor(CameraMotor targetMotor)
  {
	 _cameraController.ActivateMotor(targetMotor);
  }
  public void TransitionMotor(string nameMotor, float time)
  {
	 var motor = _cameraController.GetMotor(nameMotor);
	 TransitionMotor(motor, time);
  }

  public void TransitionMotor(CameraMotor targetMotor, float time)
  {

	 int activeMotor = _cameraController.ActiveMotorIndex;
	 int indexTarget = _cameraController.GetMotorIndex(targetMotor.Name);
	 TransitionMotor motor = _cameraController.GetMotor<TransitionMotor>("CustomTransition");
	 motor.StartMotorIndex = activeMotor;
	 motor.EndMotorIndex = indexTarget;
	 motor.TransitionTime = time;
	 _cameraController.ActivateMotor(motor);
  }

#if UNITY_EDITOR

  [SerializeField] private float _rangeShake = 0.1f;
  [SerializeField] private float _timeShake = 1;

  [ContextMenu("Shake0")]
  public void Shake()
  {
	 CameraController.Shake(_rangeShake, _timeShake);
  }

#endif

}
