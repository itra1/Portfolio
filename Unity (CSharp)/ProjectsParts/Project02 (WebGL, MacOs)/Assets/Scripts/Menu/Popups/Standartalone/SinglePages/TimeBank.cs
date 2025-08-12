using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using it.Animations;

namespace it.Main.SinglePages
{
	public class TimeBank : SinglePage
	{
		[SerializeField] private TMValueIntAnimate _amountFreeLabel;
		[SerializeField] private ImageCircleFillAmount _amountFreeDiagramm;
		[SerializeField] private TMValueIntAnimate _amountLabel;
		[SerializeField] private ImageCircleFillAmount _amountDiagramm;

		protected override void EnableInit()
		{
			base.EnableInit();
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserTimebankUpdate, UserProfileUpdate);

			FillData();
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserTimebankUpdate, UserProfileUpdate);
		}

		private void UserProfileUpdate(com.ootii.Messages.IMessage handle)
		{
			FillData();
		}

		private void FillData()
		{

			_amountFreeLabel.StartValue = 0;
			_amountLabel.StartValue = 0;
			_amountFreeLabel.EndValue = (int)UserController.User.user_profile.time_bank;
			_amountLabel.EndValue = (int)UserController.User.user_profile.time_bank_paid;

			_amountFreeDiagramm.StartValue = 0;
			_amountDiagramm.StartValue = 0;
			_amountFreeDiagramm.EndValue = ((int)UserController.User.user_profile.time_bank) / 20f;
			_amountDiagramm.EndValue = ((int)UserController.User.user_profile.time_bank_paid) > 0 ? 1 : 0;

			//_amountFreeLabel.text = ((int)UserController.User.UserProfile.TimeBankFree).ToString();
			//_amountLabel.text = ((int)UserController.User.UserProfile.TimeBankPaid).ToString();
			//_amountFreeDiagramm.fillAmount = ((int)UserController.User.UserProfile.TimeBankFree) / 20f;
			//_amountDiagramm.fillAmount = ((int)UserController.User.UserProfile.TimeBankPaid) > 0 ? 1 : 0;
			_amountLabel.GetComponent<TMPro.TextMeshProUGUI>().color = ((int)UserController.User.user_profile.time_bank_paid) <= 0 ? Color.red : Color.white;

		}

	}
}