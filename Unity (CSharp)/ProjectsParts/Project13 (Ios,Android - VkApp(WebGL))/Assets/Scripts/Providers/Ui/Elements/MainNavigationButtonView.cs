using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Providers.Ui.Elements {
	public class MainNavigationButtonView :MonoBehaviour {

		public UnityAction OnClick;

		[SerializeField] private TMP_Text _label;
		[SerializeField] private RectTransform _icone;

		private bool _isSelect;
		private float _positionYImage;

		public void Awake() {
			_label.color = new(1, 1, 1, 0);
			_positionYImage = _icone.anchoredPosition.y;
		}

		public void SetSelect(bool isSelect) {
			if (_isSelect == isSelect)
				return;
			_isSelect = isSelect;
			_label.DOColor(_isSelect ? new(1, 1, 1, 1) : new(1, 1, 1, 0), 0.2f);
			_icone.DOAnchorPosY(_isSelect ? _positionYImage + 30 : _positionYImage, 0.2f);
		}

		public void MainClick() {
			OnClick?.Invoke();
		}

	}
}
