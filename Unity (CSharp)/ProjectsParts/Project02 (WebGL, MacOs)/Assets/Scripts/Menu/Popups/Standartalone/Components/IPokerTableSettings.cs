using System;
using UnityEngine;

public interface IPokerTableSettings
{
  [SerializeField]
  void SetSettings(PokerTableSettings settings);
}

