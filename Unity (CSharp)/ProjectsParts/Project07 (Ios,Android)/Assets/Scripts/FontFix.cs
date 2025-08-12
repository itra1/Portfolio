using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontFix : MonoBehaviour {

	[ContextMenu("fix")]
  public void FontFixExec() {


    Text[] textarr = FindObjectsOfType<Text>();

    textarr.ToList().ForEach(elem => {

      Debug.Log(elem.name);

    });

  }

}
