using UnityEngine;
using UnityEngine.UI;

public class StatisticEnemyIcon : MonoBehaviour {

    public GameObject[] icons;
    public Text textPoints;

    public void SetCalues(string newText, int iconsNum) {
        textPoints.text = newText;
        foreach(GameObject itm in icons) itm.SetActive(false);
        icons[iconsNum].SetActive(true);
    }
}
