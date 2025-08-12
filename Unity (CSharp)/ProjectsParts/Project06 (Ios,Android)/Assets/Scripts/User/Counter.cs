using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.User
{

  public class Counter<T>
  {

    public event System.Action<T> OnChangeValue;
    /// <summary>
    /// Стартовое значение
    /// </summary>
    private T _startValue;
    private T _value;

    public T Value
    {
      get { return _value; }
      set
      {
        _value = value;
        Save();
        if (OnChangeValue != null)
          OnChangeValue(value);
      }
    }

    public void Initiate(string key, T startValue)
    {
      _startValue = startValue;
      Initiate(key);
    }
    public void Initiate(string key)
    {

      Key = key;
      Load();
    }
    
    private string Key { get; set; }

    public void Save()
    {
      PlayerPrefs.SetString(Key, Newtonsoft.Json.JsonConvert.SerializeObject(Value));
    }

    public void Load()
    {
      if (!PlayerPrefs.HasKey(Key))
      {
        Value = _startValue;
      }

      string pref = PlayerPrefs.GetString(Key);

      _value = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(pref);
    }

  }

}