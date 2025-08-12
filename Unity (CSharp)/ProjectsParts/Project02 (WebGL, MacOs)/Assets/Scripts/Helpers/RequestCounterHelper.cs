using System.Collections;
using UnityEngine;

namespace it.Helpers
{
	public class RequestCounterHelper : MonoBehaviour
	{
		[SerializeField] private RectTransform _body;
		[SerializeField] private TMPro.TextMeshProUGUI _valueLabel;

		private void OnEnable()
		{
			ConformValue();
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserRequestUpdate, UserRequestUpdate);
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserRequestUpdate, UserRequestUpdate);
		}

		private void UserRequestUpdate(com.ootii.Messages.IMessage handler){
			ConformValue();
		}

		private void ConformValue(){
			_body.gameObject.SetActive(UserController.Instance.Cashier.RequestNoVisible > 0);
			_valueLabel.text = UserController.Instance.Cashier.RequestNoVisible.ToString();
		}

	}
}