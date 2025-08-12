using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace it.UI.Elements
{
	public class HoverActivateButtonUI : MonoBehaviour, IPointerEnterHandler
	{
		[SerializeField] private UnityEngine.Events.UnityEvent OnHover;

		public void OnPointerEnter(PointerEventData eventData)
		{
			OnHover?.Invoke();
		}
	}
}