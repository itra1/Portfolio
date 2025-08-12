using System;
using UnityEngine;
using Game.User;

namespace UI.Battle
{

  public class TextLive: MonoBehaviour
  {

    private TMPro.TextMeshProUGUI _text;

    private TMPro.TextMeshProUGUI Text
    {
      get
      {
        if (_text == null)
          _text = GetComponent<TMPro.TextMeshProUGUI>();
        return _text;
      }
    }

    private void OnEnable()
    {
      UserHealth.OnChange += HealthChange;
      HealthChange(UserHealth.Instance.Value, UserHealth.Instance.Max);
    }

    private void OnDisable()
    {
      UserHealth.OnChange -= HealthChange;
    }

    private void HealthChange(float actualValue, float maxValue)
    {
      Text.text = String.Format("{0:#}/{1:#}", actualValue, maxValue);
    }

  }
}