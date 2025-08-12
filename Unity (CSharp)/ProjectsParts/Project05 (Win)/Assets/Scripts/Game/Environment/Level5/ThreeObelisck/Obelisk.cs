using UnityEngine;
using System.Collections;
using DG.Tweening;
using it.Game.Player.Interactions;
using it.Game.Managers;

namespace it.Game.Environment { 
  /// <summary>
  /// Обелиск
  /// </summary>
  public class Obelisk : UUIDBase, IInteractionCondition
  {
	 public UnityEngine.Events.UnityAction OnActivate;
	 [SerializeField]
	 private string crystalUuid;
	 [SerializeField]
	 private GameObject _crystal;
	 [SerializeField]
	 private Light _light;
	 [SerializeField]
	 private Material _baseMaterial;

	 private bool _isActive;
	 public bool IsActive { get => _isActive; set => _isActive = value; }

	 [ContextMenuItem("Confirm", "ConfirmActiveColor")]
	 [SerializeField]
	 [ColorUsage(false, true)]
	 private Color _activeColor;
	 [ContextMenuItem("Confirm", "ConfirmDeactiveColor")]
	 [SerializeField]
	 [ColorUsage(false,true)]
	 private Color _deactiveColor;

	 private void Start()
	 {
		Deactive();
	 }

	 [ContextMenu("Interaction")]
	 public void Interaction()
	 {
		GameManager.Instance.Inventary.Remove(crystalUuid);
		Activate();
		OnActivate?.Invoke();
	 }

	 /// <summary>
	 /// Активация
	 /// </summary>
	 [ContextMenu("Activate")]
	 public void Activate()
	 {
		_isActive = true;
		_crystal.SetActive(true);

		DOTween.To(() => _baseMaterial.GetColor("_EmissionColor"), (x) => _baseMaterial.SetColor("_EmissionColor", x), _activeColor, 1);
		_light.DOIntensity(5, 1);
	 }

	 public void Deactive()
	 {
		_isActive = false;
		_crystal.SetActive(false);
		DOTween.To(() => _baseMaterial.GetColor("_EmissionColor"), (x) => _baseMaterial.SetColor("_EmissionColor", x), _deactiveColor, 1);
		_light.DOIntensity(1, 1);
	 }

	 

	 public bool InteractionReady()
	 {
		if (IsActive)
		  return false;

		return GameManager.Instance.Inventary.ExistsItem(crystalUuid);
	 }

	 private void ConfirmDeactiveColor()
	 {
		_baseMaterial.SetColor("_EmissionColor", _deactiveColor);
	 }
	 private void ConfirmActiveColor()
	 {
		_baseMaterial.SetColor("_EmissionColor", _activeColor);
	 }

#if UNITY_EDITOR

	 [ContextMenu("CreateMaterial")]
	 private void CreateMaterial()
	 {
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		Material mat = Instantiate(renderers[0].sharedMaterial);

		for(int i = 0; i < renderers.Length; i++)
		{
		  renderers[i].sharedMaterial = mat;
		}
		_baseMaterial = mat;
		_baseMaterial.SetColor("_EmissionColor", _deactiveColor);
	 }

#endif

  }
}