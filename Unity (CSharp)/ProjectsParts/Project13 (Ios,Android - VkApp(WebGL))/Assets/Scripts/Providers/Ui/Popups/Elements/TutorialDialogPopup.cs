using Game.Common.Attributes;
using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Providers.Ui.Popups.Elements {
	[PrefabName(PopupsNames.TutorialDialog)]
	public class TutorialDialogPopup :Popup, IPointerDownHandler {

		public UnityAction OnClick;

		[SerializeField] private TMP_Text _dialogLabel;
		[SerializeField] private CanvasGroup _bodyCG;
		[SerializeField] private RectTransform _body;
		[SerializeField] private RectTransform _dialogRect;
		[SerializeField] private RectTransform _tapToContinue;
		[SerializeField] private PointerView _pointer;

		public CanvasGroup BodyCG => _bodyCG;
		public RectTransform TapToContinue => _tapToContinue;
		public PointerView Pointer => _pointer;
		public RectTransform Body => _body;

		protected override void Awake() {
			base.Awake();
			TapToContinue.gameObject.SetActive(false);
		}

		public void OnPointerDown(PointerEventData eventData) {
			OnClick?.Invoke();
		}

		public void SetDialog(string message) {
			_dialogLabel.text = message;
			_dialogRect.sizeDelta = new(_dialogRect.sizeDelta.x, _dialogLabel.preferredHeight + 140);
		}
	}
}
