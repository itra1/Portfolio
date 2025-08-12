using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;


namespace it.UI.Elements
{
	public class GraphicButtonUIMulti : ButtonUI
	{
		[SerializeField] private ImageLight[] _images;
		[SerializeField] private TMTextLight[] _texts;

		[SerializeField] private string _shader = "ButtonLightHover";

		private bool _isDown;

		[System.Serializable]
		public class ImageLight
		{
			public Image Image;
			[ColorUsage(true, true)]
			public Color HoverColor = Color.white * 1.5f;
			[ColorUsage(true, true)]
			public Color DownColor = Color.white * 1.5f;
			[ColorUsage(true, true)]
			public Color DisableColor = Color.white * 1.5f;
			[ColorUsage(true, true)]
			public Color StartColor;

		}
		[System.Serializable]
		public class TMTextLight
		{
			public TextMeshProUGUI Text;
			public FontStyles StartStyle;
			public bool IsColored;
			public bool IsStyle;
			[ColorUsage(true, false)]
			public Color HoverColor = Color.white * 1.5f;
			public FontStyles HoverStyle = FontStyles.Normal;
			[ColorUsage(true, false)]
			public Color DownColor = Color.white * 1.5f;
			public FontStyles DownStyle = FontStyles.Normal;
			[ColorUsage(true, false)]
			public Color DisableColor = Color.white * 1.5f;
			public FontStyles DisableStyle = FontStyles.Normal;
			[ColorUsage(true, false)]
			public Color StartColor;
		}


		protected override void Init()
		{
			base.Init();
			for (int i = 0; i < _images.Length; i++)
			{
				int index = i;
			//	_images[index].StartColor = _images[index].Image.color;
			//	if(_images[index].HoverColor == Color.black)
			//	_images[index].HoverColor = _images[index].StartColor * 1.5f;
			//	if (_images[index].DownColor == Color.black)
			//		_images[index].DownColor = _images[index].StartColor * 1.2f;
			//	if (_images[index].DisableColor == Color.black)
			//		_images[index].DisableColor = _images[index].StartColor * 0.7f;
				var mat = new Material(Shader.Find(_shader));
				_images[index].Image.material = mat;
			}
			//for (int i = 0; i < _texts.Length; i++)
			//{
			//	int index = i;
			//	_texts[index].StartStyle = _texts[index].Text.fontStyle;
			//		_texts[index].StartColor = _texts[index].Text.color;
			//	if (_texts[index].HoverColor == Color.black)
			//		_texts[index].HoverColor = _texts[index].StartColor * 1.5f;
			//	if (_texts[index].DownColor == Color.black)
			//		_texts[index].DownColor = _texts[index].StartColor * 1.2f;
			//	if (_texts[index].DisableColor == Color.black)
			//		_texts[index].DisableColor = _texts[index].StartColor * 0.7f;
			//}
		}

		public override void Click()
		{
			if (!interactable) return;
			base.Click();

		}


		public override void NormalState()
		{
			for (int i = 0; i < _images.Length; i++)
			{
				int index = i;
				
					DOTween.To(() => _images[index].Image.material.GetColor("_ColorHDR"), (x) => _images[index].Image.material.SetColor("_ColorHDR", x), _images[index].StartColor, 0.2f);

			}
			for (int i = 0; i < _texts.Length; i++)
			{
				int index = i;
				if (_texts[index].IsStyle)
					_texts[index].Text.fontStyle = _texts[index].StartStyle;
				if(_texts[index].IsColored)
				_texts[index].Text.color = _texts[index].StartColor;
			}
		}

		public override void HighlightedState()
		{
			for (int i = 0; i < _images.Length; i++)
			{
				int index = i;
					DOTween.To(() => _images[index].Image.material.GetColor("_ColorHDR"), (x) => _images[index].Image.material.SetColor("_ColorHDR", x), _images[index].HoverColor, 0.2f);
			}
			for (int i = 0; i < _texts.Length; i++)
			{
				int index = i;
				if (_texts[index].IsStyle)
					_texts[index].Text.fontStyle = _texts[index].HoverStyle;
				if (_texts[index].IsColored)
					_texts[index].Text.color = _texts[index].HoverColor;
			}
		}
		public override void DownState()
		{
			for (int i = 0; i < _images.Length; i++)
			{
				int index = i;
					DOTween.To(() => _images[index].Image.material.GetColor("_ColorHDR"), (x) => _images[index].Image.material.SetColor("_ColorHDR", x), _images[index].DownColor, 0.2f);
			}
			for (int i = 0; i < _texts.Length; i++)
			{
				int index = i;
				if (_texts[index].IsStyle)
					_texts[index].Text.fontStyle = _texts[index].DownStyle;
				if (_texts[index].IsColored)
					_texts[index].Text.color = _texts[index].DownColor;
			}
		}
		public override void NoInteractiveState()
		{
			for (int i = 0; i < _images.Length; i++)
			{
				int index = i;
				DOTween.To(() => _images[index].Image.material.GetColor("_ColorHDR"), (x) => _images[index].Image.material.SetColor("_ColorHDR", x), _images[index].DisableColor, 0.2f);
			}
			for (int i = 0; i < _texts.Length; i++)
			{
				int index = i;
				if (_texts[index].IsStyle)
					_texts[index].Text.fontStyle = _texts[index].DisableStyle;
				if (_texts[index].IsColored)
					_texts[index].Text.color = _texts[index].DisableColor;
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			NormalState();
		}

		public override void DisableState()
		{
			NoInteractiveState();
		}

		public override void Select()
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}