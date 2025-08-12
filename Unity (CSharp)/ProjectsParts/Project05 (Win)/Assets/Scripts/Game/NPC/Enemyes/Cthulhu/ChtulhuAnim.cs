using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Enemy.Chtulhu
{
  public class ChtulhuAnim : MonoBehaviour
  {
    
    public Animator _animator;

    float nextAnim;

    private void Update()
    {
      if (nextAnim > Time.time)
        return;
      _animator.SetInteger("Anim", Random.Range(0, 4));
      _animator.SetTrigger("Play");
      nextAnim = Time.time + Random.Range(1f, 4f);

    }
  }
}