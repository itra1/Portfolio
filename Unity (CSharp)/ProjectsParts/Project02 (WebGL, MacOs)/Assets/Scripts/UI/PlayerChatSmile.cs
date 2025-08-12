
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Inputs;
using it.Network.Rest;
using System;
using DG.Tweening;

namespace it.UI
{
	public class PlayerChatSmile : MonoBehaviour
	{
		[SerializeField] private RawImage _smile;
		[SerializeField] private AnimationCurve _animationCurve;
		[SerializeField] private AnimationCurve _animationCurveHide;

		private void OnEnable()
		{
			_smile.transform.localScale = Vector3.zero;
			_smile.transform.DOScale(Vector3.one, 0.3f).SetEase(_animationCurve).OnComplete(() =>
			{
				_smile.transform.DOScale(Vector3.zero, 0.3f).SetDelay(2).SetEase(_animationCurveHide).OnComplete(() =>
				{
					Destroy(gameObject);
				});
			});
		}

		public void SetTexture(Texture2D sprite)
		{
			_smile.texture = sprite;

			AspectRatioFitter arf = _smile.GetComponent<AspectRatioFitter>();
			arf.aspectRatio = (float)sprite.width / (float)sprite.height;
		}

	}
}