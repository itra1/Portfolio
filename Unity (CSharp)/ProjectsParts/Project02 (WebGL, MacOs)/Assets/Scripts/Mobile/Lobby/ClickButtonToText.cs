using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickButtonToText : MonoBehaviour
{
    public TMP_InputField myText;        // drag your text object on here

    public void ClickLetter(string letterClicked)
    {
          string tempCurString = myText.text;


        if (myText.text.Length != myText.characterLimit)
        {
            string tempNewString = tempCurString + letterClicked;
            myText.text = tempNewString.ToString();
        }
       
       
    }

    public void Clear()
    {
        myText.text = string.Empty;
    }
}