using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCompany.Save {
  [System.Serializable]
  public class Word {

    public string word;
    public bool isOpen;

    public List<int> hintLetters = new List<int>();
  }
}