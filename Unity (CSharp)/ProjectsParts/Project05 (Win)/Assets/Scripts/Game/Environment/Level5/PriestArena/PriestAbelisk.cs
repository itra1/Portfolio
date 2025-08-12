using UnityEngine;
using System.Collections;
using DG.Tweening;
using it.Game.Player.Interactions;

namespace it.Game.Environment.Level5.PriestArena
{
  public class PriestAbelisk : UUIDBase
  {
	 [SerializeField]
	 private Light _light;
	 private int _State;
	 public int State { get => _State; set => _State = value; }

	 [SerializeField]
	 private it.Game.Handles.RotateAround _crystalRotate;
	 [SerializeField]
	 [ColorUsage(false, true)]
	 private Color _colorReady;
	 [SerializeField]
	 [ColorUsage(false, true)]
	 private Color _colorActive;

	 [SerializeField]
	 private Transform _laser;

	 public UnityEngine.Events.UnityAction OnActivate;
	 //private bool _isActive;
	 public bool IsActive { get; set; }

	 private bool _isReady = false;
	 private Material _material;

	 private void Start()
	 {
		Renderer[] renderers = GetComponentsInChildren<Renderer>();

		_material = Instantiate(renderers[0].material);
		_material.SetColor("_EmissionColor", Color.black);
		_crystalRotate.enabled = false;

		for (int i = 0; i < renderers.Length; i++)
		{
		  renderers[i].material = _material;
		}
	 }

	 public void StartInteraction()
	 {
		Activation();
	 }

	 public void Activation()
	 {
		SetState2();
	 }

	 /// <summary>
	 /// Отключены
	 /// </summary>
	 public void SetState0()
	 {
		_State = 0;
		_material.SetColor("_EmissionColor", Color.black);
		//_material.DOColor(Color.black, "_EmissionColor", 1);
		_crystalRotate.IsActived = false;
		_light.intensity = 0;
		_laser.gameObject.SetActive(false);
		IsActive = false;
	 }

	 // Готовы к использованию
	 public void SetState1()
	 {
		_State = 1;
		_material.SetColor("_EmissionColor", _colorActive);
		//_material.DOColor(_colorReady, "_EmissionColor", 1);
		_crystalRotate.IsActived = true;
		_light.intensity = 8;
		_laser.gameObject.SetActive(true);
		IsActive = false;
	 }

	 public void SetState2()
	 {
		IsActive = true;
		_material.DOColor(Color.black, "_EmissionColor", 1).OnComplete(() =>
		{
		  _State = 2;
		  OnActivate?.Invoke();
		});
		_light.DOIntensity(0, 1);
		_laser.gameObject.SetActive(false);
	 }

  }
}