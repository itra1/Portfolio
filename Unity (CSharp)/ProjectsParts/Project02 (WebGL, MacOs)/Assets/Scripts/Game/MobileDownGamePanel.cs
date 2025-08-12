using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace it.UI
{
	public class MobileDownGamePanel : MonoBehaviour
	{
		[SerializeField] private RectTransform _contentRt;
		[SerializeField] private RectTransform _closePanel;

		private bool _isUpMove;

		private void Awake()
		{
			_closePanel.gameObject.SetActive(false);
		}

		public void MoveUpToggleTouch(){

			_isUpMove = !_isUpMove;
			if (_contentRt == null) return;
			if (!_isUpMove)
				_contentRt.DOAnchorPos(Vector2.zero, 0.3f);
			else
				_contentRt.DOAnchorPos(new Vector2(0, 360f), 0.3f);
			_closePanel.gameObject.SetActive(_isUpMove);
		}

	}
}