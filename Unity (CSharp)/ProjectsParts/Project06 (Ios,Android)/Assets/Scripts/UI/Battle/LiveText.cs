using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.User;

namespace Game.UI.Battle.UserStat
{
  public class LiveText : MonoBehaviour {
    [SerializeField]
    private TMPro.TextMeshProUGUI _text;
    
    private void OnEnable() {
      UserHealth.OnChange += HealthChange;
      HealthChange(UserHealth.Instance.Value, UserHealth.Instance.Max);
    }

    private void OnDisable() {
      UserHealth.OnChange -= HealthChange;
    }

    private void HealthChange(float actualValue, float maxValue) {
      _text.text = String.Format("{0}/{1}", actualValue, maxValue);
    }

  }
}