using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.NPC.Enemyes
{
  public class ShooterStaticFireGolem : Enemy
  {
	 [SerializeField] private Material _material;
	 [SerializeField] [ColorUsage(false, true)] private Color _activeColor;

	 private Material _currentMaterial;
	 private bool _isActive;

	 protected override void Awake()
	 {
		base.Awake();
		_isActive = false;
		CreateMaterials();
	 }

	 private void CreateMaterials()
	 {
		_currentMaterial = Instantiate(_material);
		_currentMaterial.SetColor("_EmissionColor", Color.black);

		Transform skin = transform.Find("Rig_golems/Geometry");

		Renderer[] renders = skin.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < renders.Length; i++)
		  renders[i].material = _currentMaterial;
	 }

	 public void Activate()
	 {
		if (_isActive) return;
		_isActive = true;
		_currentMaterial.DOColor(_activeColor, "_EmissionColor", 1);
	 }

	 public void Deactivate()
	 {
		if (!_isActive) return;
		_isActive = false;

		_currentMaterial.DOColor(Color.black, "_EmissionColor", 1);
	 }

  }
}