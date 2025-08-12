using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportPopup : MonoBehaviour
{
    [SerializeField] private GameObject about;
    [SerializeField] private GameObject chat;
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void ShowMail()
    {
        Application.OpenURL("https://garillapoker.com/");
    }
    
    public void ShowChat()
    {
        if (chat != null) chat.SetActive(true);
    }
    
    public void ShowRules()
    {
        Application.OpenURL("https://garillapoker.com/");
    }
    
    public void ShowPrivatePolicy()
    {
        Application.OpenURL("https://garillapoker.com/");
    }
    
    public void ShowAbout()
    {
        if (about != null) about.SetActive(true);
    }
}
