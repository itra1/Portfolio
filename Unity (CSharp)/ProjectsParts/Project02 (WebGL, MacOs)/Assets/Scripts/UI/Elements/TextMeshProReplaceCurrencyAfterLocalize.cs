using System.Collections;

using UnityEngine;

namespace it.UI.Elements
{
	public class TextMeshProReplaceCurrencyAfterLocalize : MonoBehaviour
	{
#if UNITY_IOS
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
			StartCoroutine(Replace());
		}

		IEnumerator Replace()
		{
			yield return null;

			//var rectComp = GetComponent<RectTransform>();
			var textComp = GetComponent<TMPro.TextMeshProUGUI>();

			if (textComp == null /*|| rectComp == null*/) yield break;

			string sourceText = textComp.text;

			sourceText = sourceText.Replace(StringConstants.CURRENCY_EURO, StringConstants.CURRENCY_IOS);

			textComp.text = sourceText;

		}

#endif
	}
}