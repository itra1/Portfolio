namespace Assets.Scripts.Billing {

  public class HeartVideoProduct: BillingProductAbstract {

    public int count;

    public override void Bye(bool isRestore = false) {
      base.Bye(isRestore);

      Company.Live.LiveCompany.Instance.AddValue(1);
      Company.Live.LiveCompany.Instance.Save();

    }
  }
}