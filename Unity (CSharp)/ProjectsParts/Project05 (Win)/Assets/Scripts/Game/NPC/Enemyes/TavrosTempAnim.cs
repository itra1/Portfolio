using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TavrosTempAnim : MonoBehaviourBase
{
  public Animator _animator;
  public Light _auraLight;
  public Light _eyeLight;
  public Material _eyeMat;
  [ColorUsage(false,true)]
  public Color _defEyeColor;
  [ColorUsage(false, true)]
  public Color _activeEyeColor;
  public float _speed = 5;
  private void Start()
  {
    _eyeMat.SetColor("_EmissionColor", _defEyeColor);
    isMove = false;
  }

  private bool isMove;

  public void Play()
  {
    DOTween.To(() => _auraLight.intensity, x => _auraLight.intensity = x, 15f, 1f).OnComplete(()=> {
      _animator.SetTrigger("Play");
    });
    DOTween.To(() => _eyeLight.intensity, x => _eyeLight.intensity = x, 0.85f, 1f);
    DOTween.To(() => _eyeMat.GetColor("_EmissionColor"), x => _eyeMat.SetColor("_EmissionColor", x), _activeEyeColor, 1f);
    InvokeSeconds(() => { isMove = true;
      _animator.SetTrigger("Move");
      DOTween.To(() => _auraLight.intensity, x => _auraLight.intensity = x, 5f, 3f);
    }, 5);
  }

  private void Update()
  {
    if (!isMove)
      return;

    transform.position += new Vector3(1,0,0) * Time.deltaTime * _speed;
  }

}
