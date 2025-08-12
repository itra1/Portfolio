using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriestAnim : MonoBehaviour
{
  public MaterialsData[] _materials;
  public Animator _animator;

  [ContextMenu("FindColors")]
  private void GetColors()
  {
    for(int i = 0; i < _materials.Length; i++)
    {
      _materials[i]._emitColor = _materials[i]._material.GetColor("_EmissionColor");
    }
  }

  [System.Serializable]
  public struct MaterialsData
  {
    public Material _material;
    [ColorUsage(true,true)]
    public Color _emitColor;
  }
  float deltaColor;
  bool isPlay;
  [ContextMenu("Play")]
  public void Play()
  {
    _animator.SetTrigger("Anim");
    StartCoroutine(Cor());
  }

  IEnumerator Cor()
  {

    ResetColor();
    yield return new WaitForSeconds(2);
    deltaColor = 0;
    isPlay = true;
    yield return new WaitForSeconds(10);
  }

  [ContextMenu("Stop")]
  public void ResetColor()
  {
    for (int i = 0; i < _materials.Length; i++)
    {
      _materials[i]._material.SetColor("_EmissionColor", Color.black);
    }

  }

  

  public void Update()
  {
    if (!isPlay) return;

    deltaColor += 0.5f * Time.deltaTime;

    for (int i = 0; i < _materials.Length; i++)
    {
      _materials[i]._material.SetColor("_EmissionColor", Color.Lerp(Color.black, _materials[i]._emitColor, deltaColor));
    }

    if (deltaColor >= 1)
      isPlay = false;

  }

}
