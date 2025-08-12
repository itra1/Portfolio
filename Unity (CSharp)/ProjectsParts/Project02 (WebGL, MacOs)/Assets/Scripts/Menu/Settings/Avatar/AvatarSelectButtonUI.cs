using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class AvatarSelectButtonUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private GameObject SelectBG;
    private SelectAvatarUI selectAvatarUI;
    [HideInInspector] public AvatarObject avatar;

    public void Init (AvatarObject avatar, SelectAvatarUI selectAvatarUI)
    {
        this.selectAvatarUI = selectAvatarUI;
        this.avatar = avatar;
        if (avatar != null)
        {
            image.gameObject.SetActive(true);
        
        }

        nameTxt.text = avatar.name;
    }

    private void Start()
    {
      //  image.sprite = SpriteManager.instance.avatarDefault;
        StartCoroutine(SetImage(avatar.url));
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
            image.sprite = sprite;
        }
    }

    public void Click ()
    {
        selectAvatarUI.SelectAvatar(avatar);
        SelectBG.gameObject.SetActive(false);
    }

    public void SetSelect (bool bl)
    {
        SelectBG.SetActive(bl);
    }
}
