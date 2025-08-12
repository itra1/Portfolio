using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace it.Game.Enemy.Chtulhu
{

	public class ChtulhuBehaviour : MonoBehaviour
	{
		[HideInInspector]
		public UnityEngine.AI.NavMeshAgent navMeshAgent;
		public ChtulhuNest nest;
		[HideInInspector]
		public ChtulhuAnimatorBase animator;
		public BehaviourState state = BehaviourState.idle;

		private void Awake()
		{
			animator = GetComponent<ChtulhuAnimatorBase>();
			navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		}

		private void Start()
		{
				
		}

	}

	public enum BehaviourState
	{
		walk,
		idle
	}

}