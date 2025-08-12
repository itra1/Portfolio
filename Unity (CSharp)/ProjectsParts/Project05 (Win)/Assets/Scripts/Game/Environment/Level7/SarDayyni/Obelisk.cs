using UnityEngine;
using System.Collections;
using DG.Tweening;
using it.Game.Player.Interactions;

namespace it.Game.Environment.Level7.SarDayyni
{
  public class Obelisk : MonoBehaviour, IInteractionCondition
  {
	 public System.Action onActivate;
	 [SerializeField]
	 private Renderer _renderer;

	 private bool _isActivated = false;
	 public bool IsActivated { get => _isActivated; set => _isActivated = value; }

	 public bool InteractionReady() { return !_isActivated; }

	 private void Start()
	 {
		Clear();
		Init();
	 }

	 public void Init()
	 {
		_renderer.material = Instantiate(_renderer.material);
	 }

	 [ContextMenu("Clear")]
	 public void Clear()
	 {
		_isActivated = false;
		_renderer.material.SetFloat("_MaterialLerp", 0);
	 }


	 [ContextMenu("Interaction")]
	 public void Interaction()
	 {
		Activate();
	 }

	 public void Activate()
	 {
		if (_isActivated) return;

		_isActivated = true;

		_renderer.material.DOFloat(1, "_MaterialLerp", 1);
	 }
  }
}