using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Battle.UserStat
{
  public class SilverCount : MonoBehaviour
  {

    [SerializeField] private TMPro.TextMeshProUGUI _text;

    private void OnEnable()
    {
      BattleManager.OnSilverCoins += Change;
      Change(BattleManager.Instance.silverCoins);
    }

    private void OnDisable()
    {
      BattleManager.OnSilverCoins -= Change;
    }

    private void Change(int count)
    {
      _text.text = count.ToString();
    }
  }
}