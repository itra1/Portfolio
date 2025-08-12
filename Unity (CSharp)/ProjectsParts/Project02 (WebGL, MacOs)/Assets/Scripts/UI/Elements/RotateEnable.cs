using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace it.UI.Elements
{
	public class RotateEnable : MonoBehaviour
	{
		[SerializeField] private RectTransform _arrowRect;

		private void OnEnable()
		{

			_arrowRect.DORotate(new Vector3(0, 0, 180), 0.3f);
		}

		private void OnDisable()
		{
			_arrowRect.DORotate(new Vector3(0, 0, 0), 0.3f);

		}
	}
}