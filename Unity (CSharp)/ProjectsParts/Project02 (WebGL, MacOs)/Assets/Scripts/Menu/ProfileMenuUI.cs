using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using it.Network.Rest;

public class ProfileMenuUI : MonoBehaviour
{
    [Space]
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Amount;
    public Image Avatar;

    private UserProfile userProfile;

    public void Init()
    {
        GameHelper.GetUserProfile(OpenProfile);
        var usuerInfo = GameHelper.UserInfo;
        Name.text = usuerInfo.nickname;
        Amount.text = usuerInfo.user_wallet.amount.ToString();
        Avatar.sprite = SpriteManager.instance.avatarDefault;
    }

    public void Close()
    {
        gameObject.SetActive(false);
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

    void OpenProfile (UserProfile profile)
    {
        userProfile = profile;
        if (profile.AvatarUrl != null && profile.AvatarUrl.Length > 0) StartCoroutine(SetImage(profile.AvatarUrl));
    }

}
