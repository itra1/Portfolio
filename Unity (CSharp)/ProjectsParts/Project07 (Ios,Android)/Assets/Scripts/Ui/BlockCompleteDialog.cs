using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockCompleteDialog : UiDialog {

  public System.Action OnNext;

  [SerializeField]
  private Text _baksText;
  [SerializeField]
  private Text _coinsText;
  [SerializeField]
  private Text _energyText;

  protected override void OnEnable() {
    base.OnEnable();

    SetData();
  }



  public void SetData()
  {
    Reward();
  }

  private void Reward()
  {
    int block = UserManager.Instance.ActiveLocation.Block;

    int hardCash = 100 * (block + 1);
    _baksText.text = hardCash.ToString();
    UserManager.Instance.HardCash += hardCash;

    int counsCount = 1000 * (block + 1);
    _coinsText.text = counsCount.ToString();
    UserManager.Instance.Gold += counsCount;

    int energy = 100;
    _energyText.text = energy.ToString();
    UserEnergy.Instance.energy += energy;
  }

  public void OnPlay()
  {
    OnNext?.Invoke();
  }

}
