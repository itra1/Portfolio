using UnityEngine;
public class MinMaxValueView : MinMaxInputValueViewBase
{

  [SerializeField]
  float step;
  public override void DiscreaseValue()
  {
    currentValue -= step;
    OnValueChange?.Invoke();

	}

  public override void IncreaseValue()
  {
    currentValue += step;
		OnValueChange?.Invoke();
	}

  public override string InitLabel()
  {
    var pref=HasPrefix==true? prefix:"";
    return currentValue.ToString().Replace(',','.') + pref;
  }

  public override void SetSettings(PokerTableSettings settings)
  {
    minValue = settings.MinPlayers;
    maxValue = settings.MaxPlayers;
    Init();
  }
  public override void SetSettings(float minValue, float maxValue)
  {
    this.minValue = minValue;
    this.maxValue = maxValue;
    Init();
  }
}
