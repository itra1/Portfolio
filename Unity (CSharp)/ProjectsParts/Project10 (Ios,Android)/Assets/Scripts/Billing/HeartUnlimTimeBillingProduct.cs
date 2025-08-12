namespace Assets.Scripts.Billing {

  public class HeartUnlimTimeBillingProduct: BillingProductAbstract {

    public int hour;

    public override void Bye(bool isRestore = false) {
      base.Bye(isRestore);

      Company.Live.LiveCompany.Instance.AddHourUnlim(hour);

    }
  }
}