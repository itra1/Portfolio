using UnityEngine;
using System.Collections;

namespace it.Game.NPC.Animals.LittleGhos
{
  public class LittleGhosAnimation : MonoBehaviourBase
  {

	 private Animator _animator = null;

	 public Animator Animator
	 {
		get
		{

		  if (_animator == null)
			 _animator = GetComponent<Animator>();
		  return _animator;
		}
		set => _animator = value;
	 }


	 // Максимальное значение рандомной анимации
	 // 
	 // Исключая максимальное значение
	 private int _maxRandomIdleAnim = 7;

	 // Время между анимациями бездействия
	 private RangeFloat _timeBetweenIdleAnim = new RangeFloat { Min = 2, Max = 5 };

	 protected void Start()
	 {
		PlayRandomIdleAnim();
	 }

	 /// <summary>
	 /// Запуск случайной анимации
	 /// </summary>
	 private void PlayRandomIdleAnim()
	 {
		InvokeSeconds(() =>
		{
		  Animator.SetFloat("NumIdle", (float)Random.Range(0, _maxRandomIdleAnim));
		  Animator.SetTrigger("RandomAnim");
		  PlayRandomIdleAnim();
		}, Random.Range(_timeBetweenIdleAnim.Min, _timeBetweenIdleAnim.Max));
	 }

  }
}