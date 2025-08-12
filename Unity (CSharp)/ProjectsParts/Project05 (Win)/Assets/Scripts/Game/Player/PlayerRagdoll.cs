using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace it.Game.Player
{
	public class PlayerRagdoll : MonoBehaviour
	{
		[SerializeField]
		private List<Collider> m_Colliders;
		[SerializeField]
		private List<Rigidbody> m_Rigidbodys;

		[ContextMenu("Find All Colliders")]
		private void FindAllColliders()
		{
			m_Colliders = GetComponentsInChildren<Collider>().ToList();
			m_Rigidbodys = GetComponentsInChildren<Rigidbody>().ToList();
		}

		private void Start()
		{
			Disabled();
		}

		private void Enables()
		{

		}

		private void Disabled()
		{
			foreach (var item in m_Colliders)
			{
				item.enabled = false;
			}
			foreach (var item in m_Rigidbodys)
			{
				item.useGravity = false;
				item.isKinematic = true;
			}
		}

	}

}