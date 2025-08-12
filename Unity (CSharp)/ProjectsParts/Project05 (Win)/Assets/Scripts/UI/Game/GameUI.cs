using UnityEngine;
using System.Collections;
using DarkTonic;
using it.UI.GameMenu;
using it.Game.Managers;

namespace it.UI.Game
{
  public class GameUI : UIDialog
  {
	 [SerializeField]
	 private RectTransform _interactionButton;
	 [SerializeField]
	 private TMPro.TextMeshProUGUI _textRenderer;

	 private Transform _targetInteractButton;
	 private Vector3 _offsetInteractButton;

	 protected override void OnEnable()
	 {
		SetVisibleInteractButton(null);
		base.OnEnable();
	 }
	 public void SetVisibleInteractButton(Transform targetInteract)
	 {
		SetVisibleInteractButton(targetInteract, Vector3.zero);
	 }
	 public void SetVisibleInteractButton(Transform targetInteract, Vector3 offsetInteract)
	 {
		_targetInteractButton = targetInteract;
		_offsetInteractButton = offsetInteract;

		_interactionButton.gameObject.SetActive(_targetInteractButton != null);

		if (_targetInteractButton == null)
		  return;
		Positing();
	 }

	 private void LateUpdate()
	 {
		if (_targetInteractButton == null)
		  return;
		Positing();
	 }

	 private void Positing()
	 {
		Vector3 screenPos = CameraBehaviour.Instance.Camera.WorldToScreenPoint(_targetInteractButton.position + _offsetInteractButton);
		screenPos.x -= CameraBehaviour.Instance.Camera.pixelWidth / 2;
		screenPos.y -= CameraBehaviour.Instance.Camera.pixelHeight / 2;
		_interactionButton.anchoredPosition = screenPos;
	 }

  }
}