
namespace Assets.Scripts.Billing {
  public class HeartUnlimitedBillingProduct: BillingProductAbstract {


    public override void Bye(bool isRestore = false) {
      base.Bye(isRestore);

      Company.Live.LiveCompany.Instance.isUnlimited = true;
      Company.Live.LiveCompany.Instance.Save();
    }

  }
}