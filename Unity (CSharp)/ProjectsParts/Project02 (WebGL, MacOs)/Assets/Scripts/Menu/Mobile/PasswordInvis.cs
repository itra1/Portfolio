using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class PasswordInvis : MonoBehaviour
{
    [SerializeField] private TMP_InputField passwordinput;
    [SerializeField] private Image VisibleImage;
    [SerializeField] private Image InvisibleImage;

    private bool isVisible = false;

    private void Start()
    {
        SwitchVisible(isVisible);
    }

    public void SwitchVisible ()
    {
        isVisible = !isVisible;
        SwitchVisible(isVisible);
    }

    private void SwitchVisible(bool actuallyVisible)
    {
        if (actuallyVisible)
        {
            InvisibleImage.gameObject.SetActive(false);
            VisibleImage.gameObject.SetActive(true);
            passwordinput.contentType = TMP_InputField.ContentType.Standard;
            passwordinput.Select();
        }
        else
        {
            InvisibleImage.gameObject.SetActive(true);
            VisibleImage.gameObject.SetActive(false);
            passwordinput.contentType = TMP_InputField.ContentType.Password;
            passwordinput.Select();
        }
        
    }
}
