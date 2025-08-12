using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace it.UI.Elements
{
	public class HoverLightMultiUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private bool _isPointer = true;
		[SerializeField] private string _shader = "ButtonLightHover";
		[SerializeField] private ImageLight[] _images;
		[SerializeField] private TMTextLight[] _texts;

		[System.Serializable]
		public class ImageLight
		{
			public Image Image;
			[ColorUsage(true, true)]
			public Color Light = Color.white * 1.5f;
			[ColorUsage(true, true)]
			public Color StartColor;

		}
		[System.Serializable]
		public class TMTextLight
		{
			public TextMeshProUGUI Text;
			public FontStyles HoverStyle = FontStyles.Normal;
			public FontStyles StartStyle;
			[ColorUsage(true, true)]
			public Color Light = Color.white * 1.5f;
			[ColorUsage(true, true)]
			public Color StartColor;
		}
		private void Awake()
		{
			//_animator = GetComponent<Animator>();
			Init();
		}

		private void Init()
		{
			for (int i = 0; i < _images.Length; i++)
			{
				int index = i;
				_images[index].StartColor = _images[index].Image.color;
				_images[index].Light = _images[index].StartColor * 1.5f;
				var mat = new Material(Shader.Find(_shader));
				_images[index].Image.material = mat;
			}
			for (int i = 0; i < _texts.Length; i++)
			{
				int index = i;
				_texts[index].StartStyle = _texts[index].Text.fontStyle;
				_texts[index].StartColor = _texts[index].Text.color;
			}

		}



		public void OnPointerEnter(PointerEventData eventData)
		{
			ConfirmStype(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			ConfirmStype(false);
		}

		public void ConfirmStype(bool isHover){

			for (int i = 0; i < _images.Length; i++)
			{
				int index = i;
				if(isHover)
				{
					DOTween.To(() => _images[index].Image.material.GetColor("_ColorHDR"), (x) => _images[index].Image.material.SetColor("_ColorHDR", x), _images[index].Light, 0.2f);
				}
				else
				{
					DOTween.To(() => _images[index].Image.material.GetColor("_ColorHDR"), (x) => _images[index].Image.material.SetColor("_ColorHDR", x), _images[index].StartColor, 0.2f);
				}
			}
			for (int i = 0; i < _texts.Length; i++)
			{
				int index = i;
				_texts[index].Text.fontStyle = isHover ? _texts[index].HoverStyle : _texts[index].StartStyle;
				_texts[index].Text.color = isHover ? _texts[index].Light : _texts[index].StartColor;
			}
		}

	}
}