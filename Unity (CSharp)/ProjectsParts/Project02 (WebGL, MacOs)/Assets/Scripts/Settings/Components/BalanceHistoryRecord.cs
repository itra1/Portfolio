using System.Collections;
using UnityEngine;
using TMPro;


namespace it.UI.Settings
{
	public class BalanceHistoryRecord : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _dateLabel;
		[SerializeField] private TextMeshProUGUI _descriptionLabel;
		[SerializeField] private it.Inputs.CurrencyLabel _amountLabel;
		[SerializeField] private TextMeshProUGUI _avalableLabel;
		[SerializeField] private TextMeshProUGUI _categoryLabel;

		private UserWalletTransaction _record;

		public void SetData(UserWalletTransaction record){

			_record = record;
			_dateLabel.text = _record.CreateAt.ToString("G");
			_amountLabel.SetValue(Mathf.Abs((float)_record.amount));
			_descriptionLabel.text = "-";
			_avalableLabel.text = "-";
			_categoryLabel.text = _record.user_wallet_transaction_type.title;

		}

	}
}