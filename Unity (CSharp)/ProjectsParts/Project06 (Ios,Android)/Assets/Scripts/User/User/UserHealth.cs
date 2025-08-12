using ExEvent;
using UnityEngine;

namespace Game.User {

  public class UserHealth: Singleton<UserHealth>, ISave {
    /// <summary>
    /// Событие изменения
    /// </summary>
    /// <param>Актуальное значение</param>
    /// <param>Максимальное значение</param>
    public static event System.Action<float, float> OnChange;

    private float _max;
    public float Max {
      get { return _max + (_max * 0.1f * UserManager.Instance.healthLevel.Value); }
    }

    private float _value;

    /// <summary>
    /// Текущее значение
    /// </summary>
    public float Value {
      get { return _value; }
      set {
        _value = value;

        if (_value > Max)
          _value = Max;

        if (OnChange != null)
          OnChange(_value, Max);
      }
            
    }
    /// <summary>
    /// Процентное значение
    /// </summary>
    public float Percent {
      get { return _value / _max; }
    }

    public bool IsMax {
      get { return Max <= Value; }
    }

    public void StartBattle() {
      Value = UserManager.gameMode != GameMode.crusade ? Max : _value;
    }

    public void Initiate(float maxValue) {
      _max = maxValue;
    }

    public void Save() {
      PlayerPrefs.SetFloat("userHealth", Value);
    }

    public void Load() {
      Value = PlayerPrefs.GetFloat("userHealth", 0);
    }

  }

}