using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 

namespace it.Widgets
{
	public class UsersOnlineWidgets : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _userValueLavel;

		private static int _userCount = -1;

		private void OnEnable()
		{
			StartCoroutine(RequestCor());
		}

		private IEnumerator RequestCor()
		{
			Request();
			while (true)
			{
				yield return new WaitForSeconds(60);
				Request();
			}

		}

		private void ConfirmData()
		{
			_userValueLavel.text = string.Format("main.header.playersOnline".Localized(), it.Helpers.Currency.String(_userCount, false));
			RectTransform rt = _userValueLavel.GetComponent<RectTransform>();
			rt.sizeDelta = new Vector2(_userValueLavel.preferredWidth, rt.sizeDelta.y);
		}

		private void Request()
		{
			it.Api.UserApi.UserOnlineCount((result) =>
			{
				_userCount = result;
				ConfirmData();
			}, (error) =>
			{

			});
		}

	}
}