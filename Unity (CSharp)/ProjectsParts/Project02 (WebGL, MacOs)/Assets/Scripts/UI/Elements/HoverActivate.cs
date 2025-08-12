using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace it.UI.Elements
{
	public class HoverActivate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private GameObject _hoverObject;

		private void OnEnable()
		{
			//_hoverObject.gameObject.SetActive(false);
		}
		private void OnDisable()
		{
			_hoverObject.gameObject.SetActive(false);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_hoverObject.gameObject.SetActive(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_hoverObject.gameObject.SetActive(false);
		}
	}
}