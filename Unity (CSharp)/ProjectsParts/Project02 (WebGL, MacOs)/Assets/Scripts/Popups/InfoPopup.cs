using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace it.Popups
{
	public class InfoPopup : PopupBase
	{
		public UnityEngine.Events.UnityAction OnConfirm;
		[SerializeField] private TMPro.TextMeshProUGUI _descriptionLabel;

		private System.DateTime _targetTimeDisable = System.DateTime.MinValue;

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void EnableInit()
		{
			base.EnableInit();
			_targetTimeDisable = System.DateTime.MinValue;
		}

		public InfoPopup SetDescriptionString(string desc)
		{
			_targetTimeDisable = System.DateTime.MinValue;
			_descriptionLabel.text = desc;
			_descriptionLabel.gameObject.SetActive(true);
			return this;
		}
		public InfoPopup SetTimer(float timer)
		{
			_targetTimeDisable = System.DateTime.Now.AddSeconds(timer);
			return this;
		}

		private void Update()
		{
			if (_targetTimeDisable == System.DateTime.MinValue) return;

			if(_targetTimeDisable < System.DateTime.Now)
			{
				_targetTimeDisable = System.DateTime.MinValue;
				Hide();
			}

		}

		public void ConfirmTouch()
		{
			_targetTimeDisable = System.DateTime.MinValue;
			OnConfirm?.Invoke();
			Hide();
		}
	}
}