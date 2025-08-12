using System.Collections;
using UniRx;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Level6.Forgotten
{
  public class ForgottenMob : MonoBehaviour
  {
	 [SerializeField] private Material _eyeMaterial;
	 [SerializeField] [ColorUsage(false,true)] private Color _eyeColorActive;

	 private Animator _animator;



	 private void Awake()
	 {
		_animator = GetComponent<Animator>();

	 }

	 private void Start()
	 {
		Observable.Interval(System.TimeSpan.FromSeconds(7)).Subscribe(_ =>
		{
		  Cry();
		});
	 }

	 /// <summary>
	 /// Очистка
	 /// </summary>
	 public void Clear()
	 {
		_eyeMaterial.SetColor("_EmissionColor", Color.black);
	 }

	 public void EyeActivate()
	 {
		_eyeMaterial.DOColor(_eyeColorActive, "_EmissionColor", 0.5f);
	 }

	 private void Cry()
	 {
		_animator.SetTrigger("Cry");
		CameraBehaviour.Instance.CameraController.Shake(0.2f, 3f);
	 }

  }
}