using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using it.Network.Rest;

public class MenuPlayerUI : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Bank;
    public Image Avatar;

    public void Init(TablePlayerSession tablePlayerSession)
    {

        Name.text = tablePlayerSession.user.nickname;
        Bank.text = tablePlayerSession.amount.ToString();
        Avatar.sprite = SpriteManager.instance.avatarDefault;
        if (tablePlayerSession.user.AvatarUrl != null && tablePlayerSession.user.AvatarUrl.Length > 0) StartCoroutine(SetImage(tablePlayerSession.user.AvatarUrl));
    }

    IEnumerator SetImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            it.Logger.Log(request.error);
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            Avatar.overrideSprite = sprite;
        }
    }
}
