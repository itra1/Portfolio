namespace Assets.Scripts.Billing {
	
	/// <summary>
	/// Покупка энергии
	/// </summary>
	public class EnergyBillingProduct : BillingProductAbstract {
		
		public int hour;

		public override void Bye(bool isRestore = false) {
			base.Bye(isRestore);

      //UserManager.Instance.energy.AddHour(hour);
      Company.Live.LiveCompany.Instance.AddHourUnlim(hour);

		}
	}
}
