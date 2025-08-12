using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using System;

namespace it.UI.Elements
{

	public class GraphicButtonUI : ButtonUI
	{
		[ColorUsage(true, true)]
		[SerializeField] private Color _hightLightColor = Color.white*1.5f;
		[ColorUsage(true, true)]
		[SerializeField] private Color _downColor = new Color(1 * 0.6f, 1 * 0.6f, 1 * 0.6f, 1);
		[ColorUsage(true, true)]
		[SerializeField] private Color _dizInteractable = new Color(1 * 0.5f, 1 * 0.5f, 1 * 0.5f, 1);

		[SerializeField] private string _shader = "ButtonLightHover";
		//private Animator _animator;
		[SerializeField] private Graphic _image;

		private Material _material;
		private bool _isDown;
		[ColorUsage(true,true)]
		private Color _startColor;

		public Color StartColor { get => _startColor;
		set
			{
				_startColor = value;
				DoStateTransition(currentSelectionState, false);
			}
		}

		public Graphic Image { get => _image; set => _image = value; }

		protected override void Init()
		{
			base.Init();
			if (_image == null)
				_image = GetComponent<Image>();
			_startColor = _image.color;
			InitMaterial();
			DoStateTransition(currentSelectionState, false);
		}

		private void InitMaterial(){

			_material = new Material(Shader.Find(_shader));
			_image.material = _material;
		}


		public override void Click()
		{
			if (!interactable) return;
						
			base.Click();
		}


		public override void NormalState()
		{
			if (_image == null)
				Init();
			DOTween.To(() => _image.material.GetColor("_ColorHDR"), (x) => _image.material.SetColor("_ColorHDR", x), _startColor, 0.2f);
		}

		public override void HighlightedState()
		{
			if (_image == null)
				Init();
			if (_material == null)
				InitMaterial();
			DOTween.To(() => _image.material.GetColor("_ColorHDR"), (x) => _image.material.SetColor("_ColorHDR", x), _hightLightColor, 0.2f);
		}
		public override void DownState()
		{
			if (_image == null)
				Init();
			if (_material == null)
				InitMaterial();
			DOTween.To(() => _image.material.GetColor("_ColorHDR"), (x) => _image.material.SetColor("_ColorHDR", x), _downColor, 0.2f);
		}
		public override void NoInteractiveState()
		{
			if (_image == null)
				Init();
			if (_material == null)
				InitMaterial();
			DOTween.To(() => _image.material.GetColor("_ColorHDR"), (x) => _image.material.SetColor("_ColorHDR", x), _dizInteractable, 0.2f);
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
			NormalState();
		}
	}
}





