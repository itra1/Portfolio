using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.NPC.Animals.Ghost
{
  /// <summary>
  /// Маденький призрак
  /// </summary>
  public class LittleGhost : Animal
  {

    [SerializeField]
    private ParticleSystem _particleSystem;


    [SerializeField]
    private Renderer _skin;
    [SerializeField]
    [ColorUsage(false,true)]
    private Color _color;


    public void Use()
    {
      _skin.material = Instantiate(_skin.material);
      Material mat = _skin.material;
      DOTween.To(()=> mat.GetColor("_EmissionColor"), (x) => mat.SetColor("_EmissionColor", x), _color,0.7f).OnComplete(()=> {

        GameObject part = Instantiate(_particleSystem.gameObject,null);
        part.transform.position = _particleSystem.transform.position;
        part.transform.rotation = _particleSystem.transform.rotation;
        part.transform.localScale = _particleSystem.transform.localScale;
        Destroy(part, 2f);
        ParticleSystem prt = part.GetComponent<ParticleSystem>();
        prt.Play();
        Destroy(gameObject);

      });
    }
	 

  }
}