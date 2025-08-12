using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Game.VFX
{
	public class LocalPositionImpuls : MonoBehaviour
	{
		[SerializeField]
		private float m_offset = 0.1f;
		[SerializeField]
		private float m_duration = .3f;
		[SerializeField]
		private bool m_XChange = true;
		[SerializeField]
		private bool m_YChange = true;
		[SerializeField]
		private bool m_ZChange = true;

		private void Start()
		{
			Change();
		}

		private void Change()
		{
			transform.DOLocalMove(new Vector3( m_XChange ? Random.Range(-m_offset, m_offset) : 0
						, m_YChange ? Random.Range(-m_offset, m_offset) : 0
						, m_ZChange ? Random.Range(-m_offset, m_offset) : 0), m_duration)
						.OnComplete(Change);
		}

	}

}