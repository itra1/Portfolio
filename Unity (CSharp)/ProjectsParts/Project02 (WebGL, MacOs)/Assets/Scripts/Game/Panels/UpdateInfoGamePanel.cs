using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Garilla.Games;
using DG.Tweening;

namespace it.Game.Panels
{
	public class UpdateInfoGamePanel : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private TextMeshProUGUI _counterLabel;
		public GameUIManager GameManager { get; set; }

		private void Awake()
		{
			_canvasGroup.gameObject.SetActive(false);
		}

		public void Show(int hands){
			_counterLabel.text = hands.ToString();
			_canvasGroup.alpha = 1;
			_canvasGroup.gameObject.SetActive(true);

			DOTween.To(() => _canvasGroup.alpha, (x) => _canvasGroup.alpha = x, 0, 1).OnComplete(()=> {
				_canvasGroup.gameObject.SetActive(false);
			}).SetDelay(1);
		}

	}
}