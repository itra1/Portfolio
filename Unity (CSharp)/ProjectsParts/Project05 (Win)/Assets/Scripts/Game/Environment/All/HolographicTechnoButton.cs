using UnityEngine;
using System.Collections;
using it.Game.Player.Interactions;
using DG.Tweening;

namespace it.Game.Environment.All
{
  /// <summary>
  /// Гологрифаческая кнопка
  /// </summary>
  public class HolographicTechnoButton : Environment, IInteractionCondition
  {
	 public UnityEngine.Events.UnityEvent OnActivate;

	 [SerializeField] private Renderer _boxRenderer;
	 [SerializeField] private Color _standartColor;
	 [SerializeField] private Color _activeColor;
	 [SerializeField] private Light _light;
	 [SerializeField] private float _standartIntencity;
	 [SerializeField] private float _activeIntencity;

	 public bool multiButton = false;

	 private Material _material;
		private Material Material
		{
			get
			{
				if (_material == null)
				{

					_boxRenderer.material = Instantiate(_boxRenderer.material);
					_material = _boxRenderer.material;
				}
				return _material;
			}
			set
			{
				_material = value;
			}
		}
		[ContextMenu("Interaction")]
		public void Interaction()
	 {
		DOTween.To(() => Material.GetColor("_Color"), (x) => Material.SetColor("_Color", x), _activeColor, 1).OnComplete(()=> {
		  OnActivate?.Invoke();
		});
		_light.DOIntensity(_activeIntencity, 1);
		_light.DOColor(_activeColor, 1);

		if (multiButton)
		{
		  DOTween.To(() => Material.GetColor("_Color"), (x) => Material.SetColor("_Color", x), _standartColor, 1).SetDelay(2);
		  _light.DOIntensity(_activeIntencity, 1).SetDelay(2);
		  _light.DOColor(_standartColor, 1).SetDelay(2);
		}
		else
		{
		  State = 1;
		  Save();
		}
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		if (isForce)
		{
		  if(State > 0)
		  {
			 SetActiveState();
		  }
		  else
		  {
			 SetStandartState();
		  }
		}
	 }

	 [ContextMenu("Set Statndart")]
	 private void SetStandartState()
	 {
		Material.SetColor("_Color", _standartColor);
		_light.intensity = _standartIntencity;
		_light.color = _standartColor;
	 }

	 [ContextMenu("Set Active")]
	 private void SetActiveState()
	 {
		Material.SetColor("_Color", _activeColor);
		_light.intensity = _activeIntencity;
		_light.color = _activeColor;
	 }

	 public bool InteractionReady()
	 {
		return State == 0;
	 }

	 private void OnDrawGizmosSelected()
	 {
		Gizmos.color = Color.green;
		if (OnActivate != null)
		{
		  for (int i = 0; i < OnActivate.GetPersistentEventCount(); i++)
		  {
			 Gizmos.DrawLine(transform.position, (OnActivate.GetPersistentTarget(i) as Component).transform.position);
		  }
		}

	 }

  }
}