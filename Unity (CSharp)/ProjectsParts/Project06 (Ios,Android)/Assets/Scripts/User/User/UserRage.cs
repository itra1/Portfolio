namespace Game.User
{

  public class UserRage: Singleton<UserRage>, ISave
  {

    public static event System.Action<float, float> OnChange;

    private float _max;
    public float Max
    {
      get { return _max; }
    }

    private float _value;

    /// <summary>
    /// Текущее значение
    /// </summary>
    public float Value
    {
      get { return _value; }
      set
      {
        _value = value;

        if (_value > Max)
          _value = Max;

        if (OnChange != null)
          OnChange(_value, _max);
      }
    }

    public bool IsMax
    {
      get { return Max <= Value; }
    }

    /// <summary>
    /// Процентное значение
    /// </summary>
    public float Percent
    {
      get { return _value / _max; }
    }

    public void Load()
    {
      throw new System.NotImplementedException();
    }

    public void Save()
    {
      throw new System.NotImplementedException();
    }
  }

}