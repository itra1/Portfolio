using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Enemy.Chtulhu
{
	[RequireComponent(typeof(ChtulhuBehaviour))]
	public class ChtulhuIdle : MonoBehaviourBase
	{
		private ChtulhuBehaviour m_Behaviour;

		private Vector3 m_target;

		private float m_nextTimeCheck;

		private void Awake()
		{
			m_Behaviour = GetComponent<ChtulhuBehaviour>();
		}

		private void Start()
		{
			GetNextPosition();
		}

		private void Update()
		{

			switch (m_Behaviour.state)
			{
				case BehaviourState.walk:
					if (CheckDistantion())
					{
						CheckState();
					}
					break;
				default:
					if (m_nextTimeCheck < Time.time)
					{
						CheckState();
					}
					break;
			}

			// Debug.Log(Vector3.Distance(m_target, transform.position));
			// if (CheckDistantion())
			// {
			// 	GetNestPosition();
			// }
		}
		private void OnDrawGizmosSelected()
		{

			Gizmos.DrawWireSphere(m_target, 0.1f);

		}

		private bool CheckDistantion()
		{
			return ((m_target - transform.position).sqrMagnitude < 0.3f);
		}

		private void CheckState()
		{
			m_Behaviour.state = (BehaviourState)(Random.Range(0, System.Enum.GetNames(typeof(BehaviourState)).Length + 1));
            
			switch (m_Behaviour.state)
			{
				case BehaviourState.walk:
					GetNextPosition();
					break;
				default:
					SetIdle();
					break;
			}
		}

		private void SetIdle()
		{
			m_Behaviour.navMeshAgent.isStopped = true;
			m_nextTimeCheck = Random.Range(5f, 8f);
			m_Behaviour.animator.PlayIdleAnim();
		}

		private void GetNextPosition()
		{
			m_Behaviour.navMeshAgent.isStopped = false;
			m_target = m_Behaviour.nest.GetNextPosition();
			m_Behaviour.navMeshAgent.SetDestination(m_target);
			m_Behaviour.animator.PlayWalkAnim();

		}

	}

}