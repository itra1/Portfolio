using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUiMagazin : MonoBehaviour {

  public Image image;

  public void SetBullet(int magazin, int count) {

    image.fillMethod = Image.FillMethod.Vertical;
    image.fillOrigin = 2;

    image.fillAmount = 1 - (float)count / (float)magazin;

  }


}
