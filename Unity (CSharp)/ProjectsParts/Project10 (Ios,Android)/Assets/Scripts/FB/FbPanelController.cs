/* ******************
Контроллер окна фотографии с именем
**********************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if PLUGIN_FACEBOOK
using Facebook.Unity;
#endif


public class FbPanelController : MonoBehaviour {

    public Image fbAvatar;
    public Text fbName;
    
    void OnEnable() {
        //DownloadContent();
        StartCoroutine(WaitFb());
    }

    IEnumerator WaitFb() {
        int ii = 0;
        while (ii < 240) {
            ii++;
            yield return new WaitForSeconds(0.5f);
            if (FBController.instance != null && FBController.instance.userName != "") {
                DownloadContent();
                ii = 240;
            }
        }
    }
    
    // Получаем необходимые данные
    void DownloadContent()
    {
        fbAvatar.enabled = true;
        fbName.enabled = true;

        fbName.text = FBController.instance.userName;
        StartCoroutine(DownloadUserAvatar(FBController.instance.userPictureUrl));
    }

    #if PLUGIN_FACEBOOK
    // Отображаем имя
    void GetFbInfo(IGraphResult prof)
    {
        Debug.Log(prof.RawResult);

        Dictionary<string, object> dict = (Dictionary<string, object>)prof.ResultDictionary;
        Dictionary<string, object> picture = (Dictionary<string, object>)dict["picture"];
        Dictionary<string, object> pictureData = (Dictionary<string, object>)picture["data"];

        fbName.text = dict["name"].ToString();
        StartCoroutine(DownloadUserAvatar(pictureData["url"].ToString()));
    }
    #endif

    // Ожидаем загрузки аватарки
    IEnumerator DownloadUserAvatar(string avaUrl)
    {
        WWW www = new WWW(avaUrl);
        yield return www;
        fbAvatar.GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, 40, 40), new Vector2(0, 0));
    }
    
}
