using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// контроллер диалога
/// </summary>
public class Dialog : MonoBehaviour {

    public Text textDialog;

    public string dialogText {
        set { textDialog.text = value; }
    }

    public void CloseButton() {
        gameObject.SetActive(false);
    }

}
