/* **********************
 * Контроллер одного элемента списка друзей
 * **********************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if PLUGIN_FACEBOOK
using Facebook.Unity;
#endif

public class ItemListFriendController : MonoBehaviour
{

    public Color myColor;
    public Image round;
    public Text number;                                         // Ссылка на номер
    public Image avatar;                                        // Ссылка на картинку аватар
    public Text nameText;                                       // Ссылка на имя
    public Text statCount;                                      // Ссылка на статистику


    public void IsMy() {
        number.color = myColor;
        nameText.color = myColor;
        statCount.color = myColor;
        round.color = myColor;
    }

    public void SetId(string newId)
    {
        #if PLUGIN_FACEBOOK
        FBController.FBGetUserAvatar(UpdateThisAvatar);
        #endif
    }

    public void SetPicture(string PictureUrl) {
        StartCoroutine(DownloadUserAvatar(PictureUrl));
    }

    public void SetName(string newName)
    {
        nameText.text = newName;
    }

    public void SetNumber(string newNumber)
    {
        number.text = newNumber;
    }

    public void SetStatCount(string newStatCount)
    {
        int thisCount = int.Parse(newStatCount);

        if (thisCount == 0)
            statCount.text = newStatCount.ToString(); // newStatCount.ToString();
        else
            statCount.text = string.Format("{0:### ### ###}", thisCount); // newStatCount.ToString();
    }
#if PLUGIN_FACEBOOK
    void UpdateThisAvatar(IGraphResult pict)
    {
        Dictionary<string, object> dict = (Dictionary<string, object>)((Dictionary<string, object>)pict.ResultDictionary)["data"];
        StartCoroutine(DownloadUserAvatar(dict["url"].ToString()));
    }
#endif
    IEnumerator DownloadUserAvatar(string avaUrl)
    {
        WWW www = new WWW(avaUrl);
        yield return www;
        avatar.GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, 40, 40), new Vector2(0, 0));
    }
}
