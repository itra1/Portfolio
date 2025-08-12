using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilites.Geometry;
namespace it.Game.NPC.Enemyes
{
  public class DragonSleep : MonoBehaviourBase
  {
    [SerializeField]
    private ParticleSystem _gas;

    [SerializeField]
    private float _maxCheckRadius = 15;
    [SerializeField]
    private float _incrementcheck = 1f;

    private void Start()
    {
      GetComponentInChildren<it.Game.Handles.AnimationHandler>().OnEventString.AddListener(AnimEventString);
    }

    private void EmitGas()
    {

      _gas.Play();
      com.ootii.Graphics.GraphicsManager.DrawCircle(transform.position, 5, Color.red, null, 1f);
      StartCoroutine(CheckPlay());
    }

    public void AnimEventString(string data)
    {
      if (data == "gas")
        EmitGas();
    }

    IEnumerator CheckPlay()
    {
      float radius = 0.5f;

      while (radius < _maxCheckRadius)
      {
        yield return new WaitForSeconds(0.5f);
        radius += _incrementcheck;
#if UNITY_EDITOR
        com.ootii.Graphics.GraphicsManager.DrawCircle(transform.position, radius, Color.red, null, 1);
#endif
        RaycastHit _hit;
        if (!RaycastExt.SafeCircularCast(transform.position + Vector3.up * 0.5f, Vector3.down, Vector3.up, out _hit, radius, 30, it.Game.ProjectSettings.PlayerLayerMask))
          continue;

        if (Mathf.Abs((_hit.transform.position - transform.position).y) > 1.3f)
          continue;

        Game.Managers.GameManager.Instance.UserManager.Health.Damage(this,1000);
      }
    }

  }
}