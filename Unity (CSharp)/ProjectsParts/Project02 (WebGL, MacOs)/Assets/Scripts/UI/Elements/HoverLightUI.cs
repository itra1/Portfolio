using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace it.UI.Elements
{
  public class HoverLightUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private Image _image;
		[SerializeField] private bool _isPointer = true;
		[ColorUsage(true, true)]
		[SerializeField] private Color _hightLightColor = Color.white * 1.5f;
		[SerializeField] private string _shader = "ButtonLightHover";

		private Material _material;
		private Color _startColor;

		private void OnEnable()
		{
			//_animator = GetComponent<Animator>();
			Init();
		}

		private void Init()
		{
			if (_image == null)
				_image = GetComponent<Image>();
			_startColor = _image.color;
			_material = new Material(Shader.Find(_shader));
			_image.material = _material;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			HighlightState();
			if (_isPointer)
				AppManager.SetPointerCursor();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			NormalState();
			if (_isPointer)
				AppManager.SetDefaultCursor();
		}
		public void NormalState()
		{
			if (_image == null)
				Init();
			DOTween.To(() => _image.material.GetColor("_ColorHDR"), (x) => _image.material.SetColor("_ColorHDR", x), _startColor, 0.2f);
		}

		public void HighlightState()
		{
			if (_image == null)
				Init();
			DOTween.To(() => _image.material.GetColor("_ColorHDR"), (x) => _image.material.SetColor("_ColorHDR", x), _hightLightColor, 0.2f);
		}
	}
}