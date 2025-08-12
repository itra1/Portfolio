using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Challenges.KingGameOfUr
{
  public class Chip : MonoBehaviourBase
  {

	 private System.Action _OnComplete;
	 private System.Action<string, bool> _onNextSection;
	 private KingGameOfUr _Manager;
	 private int m_position;
	 private float m_Speed = 3;

	 [SerializeField]
	 private Material _material;

	 private bool m_IsMove;

	 private List<Section> m_Path = new List<Section>();

	 public void Init(KingGameOfUr manager)
	 {
		_Manager = manager;
		SetTransparent(false);
	 }


	 public void SetTransparent(bool inTransparent)
	 {
		  DOTween.To(() => _material.GetColor("_Color"), x => _material.SetColor("_Color", x), new Color(1, 1, 1, (inTransparent?0:1)), 0.5f);
	 }

	 public void ResetPosition()
	 {
		m_IsMove = false;
		m_position = 0;
	 }

	 public void Move(List<Section> path, System.Action onComplete, System.Action<string, bool> onNextSectionMove)
	 {
		_OnComplete = onComplete;
		_onNextSection = onNextSectionMove;
		m_Path = path;
		m_IsMove = true;
		_onNextSection?.Invoke(m_Path[0].Uuid, m_Path.Count == 1);
	 }

	 private void Update()
	 {

		if (!m_IsMove)
		  return;

		Vector3 targetPosition = m_Path[0].transform.position + Vector3.up / 2;
		Vector3 velocity = (targetPosition - transform.position).normalized;
		Vector3 newPosition = transform.position + velocity * Time.deltaTime * m_Speed;

		if ((newPosition - transform.position).sqrMagnitude > (targetPosition - transform.position).sqrMagnitude)
		{
		  transform.position = targetPosition;
		  m_Path.RemoveAt(0);
		  if(m_Path.Count > 0)
			 _onNextSection?.Invoke(m_Path[0].Uuid, m_Path.Count == 1);
		}
		else
		{
		  transform.position = newPosition;
		}

		if (m_Path.Count == 0)
		{
		  m_IsMove = false;
		  _OnComplete?.Invoke();
		  //m_OnComplete = null;
		  return;
		}

	 }

	 public void SetPosition(Section target)
	 {
		transform.position = target.transform.position + Vector3.up / 2;
	 }

  }
}