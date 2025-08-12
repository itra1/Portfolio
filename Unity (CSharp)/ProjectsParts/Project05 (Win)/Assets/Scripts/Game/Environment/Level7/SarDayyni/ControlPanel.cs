using UnityEngine;
using System.Collections;
using it.Game.NPC.Enemyes.Boses.SarDayyni;
using it.Game.Player.Interactions;
using DG.Tweening;

namespace it.Game.Environment.Level7.SarDayyni
{
  public class ControlPanel : MonoBehaviour, IInteractionCondition
  {
	 public UnityEngine.Events.UnityAction onActivate;

	 [SerializeField]
	 private Light _light;
	 [SerializeField]
	 public Renderer _renderer;

	 [SerializeField]
	 [ColorUsage(false,true)]
	 private Color _emissionColor;

	 private bool _isActive;
	 public bool IsActive { get => _isActive; set => _isActive = value; }

	 private void Start()
	 {
		Clear();
		Init();
	 }

	 public bool InteractionReady() {return !_isActive; }

	 [ContextMenu("Interaction")]
	 public void Interaction()
	 {
		Activate();
	 }

	 public void Clear()
	 {
		_isActive = false;
		_light.intensity = 0;
		_renderer.material.SetColor("_EmissionColor", Color.black);
	 }

	 public void Init()
	 {
		_renderer.material = Instantiate(_renderer.material);
	 }

	 public void Activate()
	 {
		if (_isActive)
		  return;

		_light.DOIntensity(3, 1f);
		_renderer.material.DOColor(_emissionColor, "_EmissionColor", 1);
		_isActive = true;

		onActivate?.Invoke();
	 }


  }
}