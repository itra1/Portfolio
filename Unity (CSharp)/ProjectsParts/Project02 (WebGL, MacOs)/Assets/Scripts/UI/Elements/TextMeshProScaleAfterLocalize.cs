using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace it.UI.Elements
{
	[RequireComponent(typeof(RectTransform))]
	public class TextMeshProScaleAfterLocalize : MonoBehaviour
	{
		[SerializeField] private float MaxX = -1;

		private void Awake()
		{
			ConfirmLocalize();
		}

		private void Start() { }

		private void OnEnable()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.LocalizationChange, LocalizationChange);
			ConfirmLocalize();
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.LocalizationChange, LocalizationChange);
		}

		private void LocalizationChange(com.ootii.Messages.IMessage handler)
		{
			ConfirmLocalize();
		}

		private void ConfirmLocalize()
		{
			StartCoroutine(Scale());
		}

		IEnumerator Scale()
		{
			yield return null;

			var rectComp = GetComponent<RectTransform>();
			var textComp = GetComponent<TMPro.TextMeshProUGUI>();

			if(textComp == null || rectComp == null) yield break;

			rectComp.sizeDelta = new Vector2(Mathf.Min((MaxX > 0 ? MaxX : float.MaxValue) ,textComp.preferredWidth),rectComp.sizeDelta.y);


		}

	}
}