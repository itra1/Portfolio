using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Кнопка инвайта в списке друзей
/// </summary>
public class FbInvateButtonList : MonoBehaviour {
    
    void OnEnable() {
        ChangeLanuage();
    }

    #region Lanuage
    
    public Text friendsInviteFb;

    void ChangeLanuage() {
        friendsInviteFb.text = LanguageManager.GetTranslate("fb_FriendsInvite");
        float width = friendsInviteFb.preferredWidth;
        friendsInviteFb.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(width+150, friendsInviteFb.transform.parent.GetComponent<RectTransform>().sizeDelta.y);
    }

    #endregion

}
