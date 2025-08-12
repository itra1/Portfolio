using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ActionTimeItemView : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI label;
  private int _value;
  public int Value{
    get{
      return _value;
    }
    set{
      _value = value;
      label.text = value.ToString() ;
    }
  }

 
}
