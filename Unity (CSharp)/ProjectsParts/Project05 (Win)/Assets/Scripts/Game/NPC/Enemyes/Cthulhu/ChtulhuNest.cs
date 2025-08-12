using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Enemy.Chtulhu
{

	public class ChtulhuNest : MonoBehaviourBase
	{
		[SerializeField]
		private float m_Radius = 4;
		[SerializeField]
		private float m_heightStartCheck = 5;

		private void OnDrawGizmosSelected()
		{

			Gizmos.DrawWireSphere(transform.position, m_Radius);
			Gizmos.DrawWireCube(transform.position + Vector3.up * m_heightStartCheck, new Vector3(1, 0.01f, 1) * m_Radius);

		}

		[ContextMenu("Check Hit")]
		public Vector3 GetNextPosition()
		{

			while (true)
			{
				Ray ray = new Ray(transform.position + ((new Vector3(-1 + Random.value * 2, 0, -1 + Random.value * 2)).normalized * (m_Radius*Random.value)) + (Vector3.up * m_heightStartCheck), Vector3.down * m_heightStartCheck * 2);

				RaycastHit hit;

				if (Physics.Raycast(ray, out hit))
				{
					if (hit.point.y <= 25.5f)
						continue;

					return hit.point;
				}
			}

		}

	}

}