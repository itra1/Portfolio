using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crossword {
  [System.Serializable]
  public class Word {

    public string word;
    public List<Letter> letterList = new List<Letter>();

    public Orientation orientation;

    public void Print() {

      string data = word;

      letterList.ForEach(let => {
        data += " " + let.position.ToString();
      });
      
    }

  }

  public enum Orientation {
    horizontal = 0,
    vertical = 1
  }

}