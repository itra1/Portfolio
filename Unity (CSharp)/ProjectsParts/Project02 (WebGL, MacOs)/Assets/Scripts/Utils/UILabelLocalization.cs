 
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILabelLocalization : MonoBehaviour
{
    [SerializeField] private string Key;
    [SerializeField] private bool IsUpperCase;
    [SerializeField] private TextMeshProUGUI label;

    [Space, Header("Editing")]
    [SerializeField] private string addTag = "";
    [SerializeField] private bool replaceName;

    private void Start()
    {
        if (label == null) label = GetComponent<TextMeshProUGUI>();
        label.text = addTag;
        label.text += IsUpperCase ? Key.Localized().ToUpper() : Key.Localized();
        if(replaceName) label.text = label.text.Replace("NAME", GameHelper.UserInfo.nickname);
    }
}