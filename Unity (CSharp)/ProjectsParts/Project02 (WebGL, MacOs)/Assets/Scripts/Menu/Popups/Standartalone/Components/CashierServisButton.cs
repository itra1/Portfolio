using it.Network.Rest;
using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace it.Popups
{
	public class CashierServisButton : MonoBehaviour
	{
		private UnityEngine.Events.UnityAction<ICashierMethod> _onClick;
		//[SerializeField] private it.UI.Elements.ObjectChangeButtonUI
		[SerializeField] private TextMeshProUGUI _idLabel;
		[SerializeField] private RawImage _logo;

		private ICashierMethod _service;

		public void Set(ICashierMethod service, string imageURL, UnityEngine.Events.UnityAction<ICashierMethod> onClick)
		{
			_onClick = onClick;
			_service = service;

			_idLabel.text = "#" + _service.Id.ToString();

			if(!string.IsNullOrEmpty(imageURL))
			{
				it.Managers.NetworkManager.Instance.RequestTexture(imageURL, (s, b) =>
				{
					_logo.texture = s;
					//_logo.GetComponent<AspectRatioFitter>().aspectRatio = (float)s.width / (float)s.height;

				}, (err) =>
				{
				});
			}

		}

		public void ThisTouch()
		{
			_onClick?.Invoke(_service);
		}

	}
}