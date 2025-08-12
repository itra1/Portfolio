using System.Collections;

using UnityEngine;
using UnityEngine.VFX;
using DG.Tweening;

namespace it.Game.Environment.Level6.Forgotten
{
  public class ForgottenObelisk : MonoBehaviour
  {
	 public System.Action OnActivate;
	 public bool IsActivate { get; private set; }

	 [SerializeField] private Transform _renderersParent;

	 private void Awake()
	 {
		InstantioateMaterials();
		InitCrystal();
	 }

	 private void InstantioateMaterials()
	 {
		Renderer[] rend = _renderersParent.GetComponentsInChildren<Renderer>();
		for(int i = 0; i < rend.Length; i++)
		{
		  rend[i].material = Instantiate(rend[i].material);
		}
	 }

	 /// <summary>
	 /// Очистка состояния обелиска
	 /// </summary>
	 [ContextMenu("Clear")]
	 public void Clear()
	 {
		IsActivate = false;
		ClearCrystal();
	 }

	 public void Activate()
	 {
		IsActivate = true;
		OnActivate?.Invoke();
	 }

	 [ContextMenu("Interaction")]
	 public void Interaction()
	 {
		CrystalActivate();
		DOVirtual.DelayedCall(1, Activate);
	 }

	 #region Crystal

	 [SerializeField] private Transform _crystal;
	 [SerializeField] [ColorUsage(false, true)]
	 private Color _colorCrystalActive;
	 private Vector3 _startPosition;
	 private float _offsetY = 0.7f;

	 private it.Game.Handles.RotateAround _rotatorCrystal;
	 private Light _crystalLight;
	 private float _clistalLightIntensity;
	 [ColorUsage(false,true)]
	 private Color _colorCrystalDisactive;
	 private Renderer _crystalRenderer;
	 private VisualEffect _vfxCrystal;

	 private void InitCrystal()
	 {
		_crystalLight = _crystal.GetComponentInChildren<Light>();
		_clistalLightIntensity = _crystalLight.intensity;

		_crystalRenderer = _crystal.GetComponent<Renderer>();
		_crystalRenderer.material = Instantiate(_crystalRenderer.material);
		_colorCrystalDisactive = _crystalRenderer.material.GetColor("_EmissionColor");

		_rotatorCrystal = _crystal.GetComponent<it.Game.Handles.RotateAround>();
		_rotatorCrystal.enabled = false;
		_startPosition = _crystal.position;

		_vfxCrystal = _crystal.GetComponentInChildren<VisualEffect>();
	 }

	 private void ClearCrystal()
	 {
		_crystal.position = _startPosition;
		_rotatorCrystal.enabled = false;
		_rotatorCrystal.Rotateion = Vector3.zero;
		_crystalRenderer.material.SetColor("_EmissionColor", _colorCrystalDisactive);
		_crystalLight.intensity = _clistalLightIntensity;
		_vfxCrystal.SendEvent("OnStop");
	 }
	 private void CrystalActivate()
	 {
		_crystal.DOMoveY(_startPosition.y + _offsetY, 1f);
		_rotatorCrystal.enabled = true;
		DOTween.To(() => _rotatorCrystal.Rotateion, (x) => _rotatorCrystal.Rotateion = x, new Vector3(0, 90, 0), 1);
		_crystalRenderer.material.DOColor(_colorCrystalActive, "_EmissionColor", 1);
		_crystalLight.DOIntensity(_clistalLightIntensity + 3, 1);
		_vfxCrystal.SendEvent("OnPlay");
	 }

	 #endregion

  }
}