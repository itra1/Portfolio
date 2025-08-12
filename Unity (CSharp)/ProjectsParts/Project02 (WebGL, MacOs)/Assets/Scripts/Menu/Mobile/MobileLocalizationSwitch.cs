using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
using UnityEngine.SceneManagement;

public class MobileLocalizationSwitch : MonoBehaviour
{
    [SerializeField] private Button[] LangSwitches;


    SystemLanguage currentLanguage = SystemLanguage.English;

    private SystemLanguage[] langOptions = new SystemLanguage[]
    {
        SystemLanguage.German,
        SystemLanguage.English,
        SystemLanguage.Spanish,
        SystemLanguage.Russian
    };

    private void Awake()
    {
        for (int a = 0; a < langOptions.Length; a++) {
            if (Application.systemLanguage == langOptions[a])
            {
                ChangeLanguage(a);
            }
        }
    }

    public void ChangeLanguage (int theValue)
    {
        for (int a = 0; a < LangSwitches.Length; a++)
        {
            
            if (a == theValue)
            {
                LangSwitches[a].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                
                LangSwitches[a].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        currentLanguage = langOptions[theValue];
    }
    
    public void ApplyLang ()
    {
        LocalizationManager.Instance.LoadLocalization(currentLanguage.ToString());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
