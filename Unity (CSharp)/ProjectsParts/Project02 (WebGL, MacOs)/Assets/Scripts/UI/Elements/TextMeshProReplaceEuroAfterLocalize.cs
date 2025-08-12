using System.Collections;

using UnityEngine;

namespace it.UI.Elements
{
	public class TextMeshProReplaceEuroAfterLocalize : MonoBehaviour
	{
		[SerializeField] private StrType _type = StrType.Short;
		[System.Flags]
		public enum StrType
		{
			None = 0,
			Short = 1,
			Long = 2
		}

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

			var textComp = GetComponent<TMPro.TextMeshProUGUI>();

			if (textComp == null) yield break;

			string sourceText = textComp.text;

			if ((_type & StrType.Short) != 0)
			{
				sourceText = sourceText.ToLower().Replace(StringConstants.CURRENCY_EURO_STR_SHORT, StringConstants.CURRENCY_IOS_STR_SHORT);
			}
			if ((_type & StrType.Long) != 0)
			{
				sourceText = sourceText.ToLower().Replace(StringConstants.CURRENCY_EURO_STR, StringConstants.CURRENCY_IOS_STR);
			}

			textComp.text = sourceText;

		}

#endif
	}
}