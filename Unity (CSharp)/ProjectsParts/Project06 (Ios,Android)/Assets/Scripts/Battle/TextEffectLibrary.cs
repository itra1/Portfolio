using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffectLibrary : Singleton<TextEffectLibrary>
{

  [SerializeField]
  private Sprite[] _sprites;

  private void Start() {
    _sprites = Resources.LoadAll<Sprite>("TextEffect/");
  }

  public Sprite GetSprite(string name) {
    foreach (var elem in _sprites)
      if (elem.name == name)
        return elem;
    return null;
  }

  protected override void OnDestroy() {
    base.OnDestroy();
    foreach (var elem in _sprites)
      Resources.UnloadAsset(elem);
  }



}
