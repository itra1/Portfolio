using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using it.Animations;

namespace it.UI
{
	public class UserProfileTimeBank : MonoBehaviour
	{

		[SerializeField] private TMValueIntAnimate _amountFreeLabel;
		[SerializeField] private ImageCircleFillAmount _amountFreeDiagramm;
		[SerializeField] private TMValueIntAnimate _amountLabel;
		[SerializeField] private ImageCircleFillAmount _amountDiagramm;

		private void OnEnable()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserTimebankUpdate, UserProfileUpdate);

			FillData();
		}

		public void ExpandButtonTouch()
		{
			it.Main.SinglePageController.Instance.Show(SinglePagesType.Timebank);
		}
		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserTimebankUpdate, UserProfileUpdate);
		}

		private void UserProfileUpdate(com.ootii.Messages.IMessage handle)
		{
			if (UserController.User == null) return;
			FillData();
		}

		private void FillData()
		{
			if (UserController.User == null || UserController.User.user_profile == null) return;
			try
			{
				_amountFreeLabel.StartValue = 0;
				_amountLabel.StartValue = 0;
				_amountFreeLabel.EndValue = (int)UserController.User.user_profile.time_bank;
				_amountLabel.EndValue = (int)UserController.User.user_profile.time_bank_paid;
				//_amountFreeLabel.text = ((int)UserController.User.UserProfile.TimeBankFree).ToString();
				//_amountLabel.text = ((int)UserController.User.UserProfile.TimeBankPaid).ToString();
				_amountFreeDiagramm.StartValue = 0;
				_amountDiagramm.StartValue = 0;
				_amountFreeDiagramm.EndValue = ((int)UserController.User.user_profile.time_bank) / 20f;
				_amountDiagramm.EndValue = ((int)UserController.User.user_profile.time_bank_paid) > 0 ? 1 : 0;

				//_amountFreeDiagramm.fillAmount = ((int)UserController.User.UserProfile.TimeBankFree) / 20f;
				//_amountDiagramm.fillAmount = ((int)UserController.User.UserProfile.TimeBankPaid) > 0 ? 1 : 0;
				_amountLabel.GetComponent<TextMeshProUGUI>().color = ((int)UserController.User.user_profile.time_bank_paid) <= 0 ? Color.red : Color.white;
			}
			catch
			{
				it.Logger.LogError("ошибка заполнения таймбанка пользователя. Состояние пользователя " + Newtonsoft.Json.JsonConvert.SerializeObject(UserController.User.user_profile));
			}

		}
	}
}