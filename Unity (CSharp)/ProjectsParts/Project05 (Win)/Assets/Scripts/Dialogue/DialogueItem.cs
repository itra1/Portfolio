using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace it.Dialogue
{
  [System.Serializable]
  public class DialogueItem
  {
	 [SerializeField]
	 private SideBlockType _sideBlock;
	 [SerializeField]
	 private Sprite _icone;
	 [SerializeField]
	 private Color _iconColor = new Color32(255,255,255,255);
	 [SerializeField]
	 private Vector2 _offset;
	 [SerializeField]
	 private string _textLangId;

	 public SideBlockType SideBlock { get => _sideBlock; set => _sideBlock = value; }
	 public Sprite Icone { get => _icone; set => _icone = value; }
	 public Vector2 Offset { get => _offset; set => _offset = value; }
	 public string TextLangId { get => _textLangId; set => _textLangId = value; }
	 public Color IconColor { get => _iconColor; set => _iconColor = value; }

	 public enum SideBlockType
	 {
		left,
		right
	 }
  }
}