using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CatDialog : MonoBehaviour {

  public Text textInfo;

  public GameObject dialogObject;
  public GameObject regionObject;
  public GameObject pointerInstance;
  
  public System.Action OnTapRegion;
  public System.Action OnTapDisplay;

  public void DisplayButton() {
    if(OnTapDisplay != null) OnTapDisplay();
  }

  public void RegionButton() {
    if(OnTapRegion != null) OnTapRegion();
  }

}
