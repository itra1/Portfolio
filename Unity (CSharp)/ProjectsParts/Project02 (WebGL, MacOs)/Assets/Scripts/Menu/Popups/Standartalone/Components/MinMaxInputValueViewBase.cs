using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class MinMaxInputValueViewBase : MonoBehaviour,IPokerTableSettings
{
  public UnityEngine.Events.UnityAction OnValueChange;

  [SerializeField]
  private Sprite minEnabledSprite;
  [SerializeField]
  private Sprite minDisabledSprite;
  [SerializeField]
  private Sprite maxEnabledSprite;
  [SerializeField]
  private Sprite maxDisabledSprite;

  [SerializeField]
  public float minValue=0;
  [SerializeField]
  public float maxValue=6;

  [SerializeField]
  private Image targetMinImage;
  [SerializeField]
  private Image targetMaxImage;
  [SerializeField]
  private TextMeshProUGUI text;

  public float currentValue;
  public bool HasPrefix = false;
  public string prefix;
  public abstract void SetSettings(PokerTableSettings settings);
  public abstract void SetSettings(float minValue, float maxValue);
  private void Start()
  {
    Init();
  }
  public virtual void Init(){
    if (currentValue < minValue)
      currentValue = minValue;
    if (currentValue > maxValue)
      currentValue = maxValue;
    if (currentValue > minValue)
    {
      targetMinImage.sprite = minEnabledSprite;
    }
    else
    {
      targetMinImage.sprite = minDisabledSprite;
    }
    if (currentValue<maxValue)
    {
      targetMaxImage.sprite = maxEnabledSprite;
    }
    else
    {
      targetMaxImage.sprite = maxDisabledSprite;
    }
    text.text = InitLabel();
  }
  public virtual void Increase(){
    if (currentValue<maxValue)
      IncreaseValue();
    Init();
    //it.Logger.Log("++");
  }
  public virtual void Discrease(){

    if (currentValue>minValue)
      DiscreaseValue();
    Init();
    //it.Logger.Log("--");
  }
  public abstract void IncreaseValue();
  public abstract void DiscreaseValue();
  public abstract string InitLabel();


}
