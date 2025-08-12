using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
  [SerializeField]
  private SpriteRenderer _sr;

  [SerializeField]
  private string _spriteName;

  private void OnEnable() {

    if (_sr.sprite == null)
      _sr.sprite = TextEffectLibrary.Instance.GetSprite(_spriteName);
  }

  [ContextMenu("Find component")]
  public void FindComponen() {

    _sr = GetComponentInChildren<SpriteRenderer>();
    _spriteName = _sr.sprite.name;

  }

}
