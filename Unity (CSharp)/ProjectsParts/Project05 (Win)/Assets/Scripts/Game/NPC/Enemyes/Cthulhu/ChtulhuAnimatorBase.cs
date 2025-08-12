using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Enemy.Chtulhu
{
  public class ChtulhuAnimatorBase : MonoBehaviourBase
  {
	 private ChtulhuBehaviour m_Behaviour;
	 protected Animator m_Animator;
	 protected string m_EarAnimParametr = "Ear";
	 protected string m_BeardAnimParametr = "Beard";
	 protected string m_BeardNumAnimParametr = "BeardNum";
	 protected string m_WalkAnimParametr = "Walk";
	 protected string m_IdleAnimParametr = "Idle";
	 protected string m_IdleNumAnimParametr = "IdleNum";
	 protected string m_EyeAnimParametr = "Eye";
	 protected string m_EyeNumAnimParametr = "EyeNum";

	 protected void Awake()
	 {
		m_Animator = GetComponent<Animator>();
		m_Behaviour = GetComponent<ChtulhuBehaviour>();
	 }

	 private void Start()
	 {
		StartCoroutine(IdledAnim());
		StartCoroutine(EarAnim());
		StartCoroutine(BeardAnim());
		StartCoroutine(EyeAnim());
	 }

	 IEnumerator EarAnim()
	 {
		while (true)
		{
		  yield return new WaitForSeconds(Random.Range(1f, 3f));
		  m_Animator.SetTrigger(m_EarAnimParametr);
		}
	 }

	 IEnumerator IdledAnim()
	 {
		while (true)
		{
		  yield return new WaitForSeconds(Random.Range(10f, 15f));
		  m_Animator.SetInteger(m_IdleNumAnimParametr, Random.Range(0, 4));
		  m_Animator.SetTrigger(m_IdleAnimParametr);
		}
	 }

	 IEnumerator BeardAnim()
	 {
		while (true)
		{
		  yield return new WaitForSeconds(Random.Range(3f, 6f));
		  m_Animator.SetInteger(m_BeardNumAnimParametr, Random.Range(0, 6));
		  m_Animator.SetTrigger(m_BeardAnimParametr);
		}
	 }

	 IEnumerator EyeAnim()
	 {
		while (true)
		{
		  yield return new WaitForSeconds(Random.Range(5f, 8f));
		  m_Animator.SetFloat(m_EyeNumAnimParametr, Random.value);
		  m_Animator.SetTrigger(m_EyeAnimParametr);
		}
	 }

	 public void PlayIdleAnim()
	 {
		m_Animator.SetBool(m_WalkAnimParametr, false);
		m_Animator.SetInteger(m_IdleNumAnimParametr, Random.Range(0, 4));
		m_Animator.SetTrigger(m_IdleAnimParametr);
	 }

	 public void PlayWalkAnim()
	 {
		m_Animator.SetBool(m_WalkAnimParametr, true);
	 }

	 /// <summary>
	 /// Событие анимации, шаг
	 /// </summary>
	 /// <param name="foot">Название шага (left|right)</param>
	 public void Step(string foot)
	 {

	 }
  }
}
