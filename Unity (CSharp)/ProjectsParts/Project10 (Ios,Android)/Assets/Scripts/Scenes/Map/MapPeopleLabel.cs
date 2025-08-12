using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if PLUGIN_FACEBOOK
using Facebook.Unity;
#endif

public class MapPeopleLabel: MonoBehaviour {

  public MeshRenderer[] meshes;
  public SpriteRenderer[] sprites;
  public TextMesh[] textMeshes;
  [HideInInspector] public string facebookId;

  float[] meshTransperent;
  float[] spriteTransperent;
  float[] textTransperent;

  float diffValue;
  float difffull;

  [HideInInspector] public string avatarUrl;
  [HideInInspector] public bool awaWait;

  void OnEnable() {

    MapController.swipe += CheckColor;
    awaWait = false;

    InitColor();
    CheckColor();
  }

  void OnDisable() {
    StopCoroutine(DownloadAllSprite());
    StopAllCoroutines();
    MapController.swipe -= CheckColor;
  }

  void OnDestroy() {
    StopCoroutine(DownloadAllSprite());
    StopAllCoroutines();
  }

  public void GetAvatar(string facebookId) {
#if PLUGIN_FACEBOOK
        FBController.FBGetFriendAvatar(facebookId,SetAvatar);
#endif
  }
#if PLUGIN_FACEBOOK
    void SetAvatar(IGraphResult result) {
        
        Dictionary<string, object> dict = (Dictionary<string, object>)((Dictionary<string, object>)result.ResultDictionary)["data"];
        avatarUrl = dict["url"].ToString();
        if(gameObject)
        StartCoroutine(DownloadAllSprite());

    }
#endif

  /// <summary>
  /// Запоминаем текущее значение прозрачности, и устанавливаем его на 0
  /// </summary>
  void InitColor() {

    meshTransperent = new float[meshes.Length];
    spriteTransperent = new float[sprites.Length];
    textTransperent = new float[textMeshes.Length];

    for (int i = 0; i < meshes.Length; i++) {
      meshTransperent[i] = meshes[i].material.color.a;
      meshes[i].material.color = new Color(meshes[i].material.color.r, meshes[i].material.color.g, meshes[i].material.color.b, 0);
    }

    for (int i = 0; i < sprites.Length; i++) {
      spriteTransperent[i] = sprites[i].color.a;
      sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 0);
    }

    for (int i = 0; i < textMeshes.Length; i++) {
      textTransperent[i] = textMeshes[i].color.a;
      textMeshes[i].color = new Color(textMeshes[i].color.r, textMeshes[i].color.g, textMeshes[i].color.b, 0);
    }

    float pixelSize = CameraController.displayDiff.right * 2 / Display.main.systemWidth;

    CheckColor();

    diffValue = Display.main.systemWidth / 4 * pixelSize;
    difffull = diffValue / 2 * pixelSize;

  }

  /// <summary>
  /// Проверяем степень удаления от центра, и делаем лейбл видимым
  /// </summary>
  void CheckColor() {
    float camPosX = 0;

    try {
      camPosX = CameraController.displayDiff.transform.position.x;
    } catch {
      camPosX = transform.position.x;
    }

    float thisDiff = Mathf.Abs(camPosX - transform.position.x);
    if (thisDiff > diffValue && transform.position.x < 8) return;

    float setTransperent = 0;


    if (thisDiff <= difffull) {
      setTransperent = 1;
    } else if (thisDiff <= diffValue) {
      setTransperent = 1 - ((thisDiff - difffull) / (diffValue - difffull));
    } else {
      setTransperent = 0;
    }

    if (transform.position.x > 8) setTransperent = 1;

    for (int i = 0; i < meshes.Length; i++) {
      meshes[i].material.color = new Color(meshes[i].material.color.r, meshes[i].material.color.g, meshes[i].material.color.b, meshTransperent[i] * setTransperent);
    }

    for (int i = 0; i < sprites.Length; i++) {
      sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, spriteTransperent[i] * setTransperent);
    }

    for (int i = 0; i < textMeshes.Length; i++) {
      textMeshes[i].color = new Color(textMeshes[i].color.r, textMeshes[i].color.g, textMeshes[i].color.b, textTransperent[i] * setTransperent);
    }
    ChangeLayer(thisDiff);
  }

  void ChangeLayer(float diff) {
    int nevOrder = 10000 - (int)(1000 * diff);

    for (int i = 0; i < meshes.Length; i++) {
      meshes[i].sortingOrder = nevOrder + 5 + i;
    }

    for (int i = 0; i < sprites.Length; i++) {
      sprites[i].sortingOrder = nevOrder + 2 + i;
    }

    for (int i = 0; i < textMeshes.Length; i++) {
      textMeshes[i].GetComponent<MeshRenderer>().sortingOrder = nevOrder + 3 + i;
    }
  }

  IEnumerator DownloadAllSprite() {

    for (int i = 0; i < meshes.Length; i++) {
      WWW www = new WWW(avatarUrl);
      yield return www;
      meshes[i].material.mainTexture = www.texture;
      yield return new WaitForSeconds(0.2f);
    }
  }

}
