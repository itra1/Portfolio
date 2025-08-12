using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Configuration.Survivles {

  public class Item {

    public float bat;
    public float blackMark;
    public int boxes;
    public float dino;
    public string distantion;
    public float hearth;
    public float magic;
    public float magnet;
    public float objects;
    public float spider;
    public float trap;

    public float dist {
      get {

        float res = 0;

        if (float.TryParse(distantion, out res)) {
          return res;
        } else {
          return 0;
        }
      }
    }

  }
}