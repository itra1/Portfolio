namespace Assets.Scripts.Billing {



	/// <summary>
	/// Покупка золота
	/// </summary>
	public class GoldBillingProduct : BillingProductAbstract {

		public int count;

		public override void Bye(bool isRestore = false) {
			base.Bye(isRestore);

			UserManager.coins += count;

		}

	}

}