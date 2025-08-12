using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Scenes
{
  public class Level7LocationManager : LocationManager
  {
	 public override int LevelIndex => 7;

	 public Color ColorDefault { get => _colorDefault; set => _colorDefault = value; }
	 public Color ColorBlocked { get => _colorBlocked; set => _colorBlocked = value; }
	 public Color ColorOpen { get => _colorOpen; set => _colorOpen = value; }

	 [SerializeField]
	 [ColorUsage(false,true)]
	 private Color _colorDefault;
	 [SerializeField]
	 [ColorUsage(false, true)]
	 private Color _colorBlocked;
	 [SerializeField]
	 [ColorUsage(false, true)]
	 private Color _colorOpen;

  }
}