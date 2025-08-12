using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment
{
  public class StonePortal : Environment
  {
    public Material _stoneMat;


    // Start is called before the first frame update
    protected override void Start()
    {
      base.Start();
      _stoneMat.SetColor("_Color", new Color(1, 1, 1, 0));
    }

    public void Activate()
    {
      DOTween.To(() => _stoneMat.GetColor("_Color"), (x) => _stoneMat.SetColor("_Color", x), new Color(1, 1, 1, 1), 1f).OnComplete(()=> {

        GetComponent<Animator>().SetTrigger("Show");
      
      });
    }
  }
}