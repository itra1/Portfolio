using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class elementInDailyBonus : MonoBehaviour {

    public DailyBonus daily;

    public GameObject backSprite;
    public Text titleText;
    public Text countText;

    void OnEnable() {
        Resize();
    }

    public void HideElement() {
        GetComponent<Animator>().SetTrigger("hide");
    }
    
    public void Resize() {
        float widthAll = Mathf.Max(titleText.preferredWidth,countText.preferredWidth) + 250;
        //backSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(widthAll, backSprite.GetComponent<RectTransform>().sizeDelta.y);
        GetComponent<RectTransform>().sizeDelta = new Vector2(widthAll, GetComponent<RectTransform>().sizeDelta.y);
    }
}
