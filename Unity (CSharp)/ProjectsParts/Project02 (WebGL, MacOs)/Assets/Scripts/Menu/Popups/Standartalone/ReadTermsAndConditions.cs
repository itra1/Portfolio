using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadTermsAndConditions : MonoBehaviour
{
    [SerializeField] Sprite activeButtonSprite;
    [SerializeField] Sprite inActiveButtonSprite;
    [SerializeField] Toggle toggle;
    [SerializeField] Button confirm;
    [SerializeField] TMP_Text text;

    private Sprite _sprite;

    private void OnEnable()
    {
        toggle.isOn = false;
        confirm.interactable = false;

        toggle.onValueChanged.AddListener((value) =>
        {

            if (value)
            {
                confirm.interactable = true;
                confirm.GetComponent<Image>().sprite = activeButtonSprite;
                text.color = Color.black;
            }
            else
            {
                confirm.interactable = false;
                confirm.GetComponent<Image>().sprite = inActiveButtonSprite;
                text.color = Color.white;
            }
        });

        confirm.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });
    }

    public void LoadUrl()
    {
        Application.OpenURL("https://garillapoker.com/terms-conditions");
    }

}
  